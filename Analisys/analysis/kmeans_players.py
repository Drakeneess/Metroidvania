# analysis/kmeans_players.py
import pandas as pd
import numpy as np
from sklearn.preprocessing import StandardScaler
from sklearn.cluster import KMeans
from sklearn.metrics import silhouette_score

from modules.session_logs import get_playthroughs_with_logs
from clean.cleaner import clean_log_dataframe
from clean.feature_engineering import extract_behavior_features

# =========================
# Construcción de features a nivel SESIÓN
# =========================
def _build_session_features(limit=10000) -> pd.DataFrame:
    """
    Construye un DataFrame por SESIÓN combinando:
      - features avanzadas extraídas del log (feature_engineering)
      - metadata desde vw_playthrough_summary (si existe)
    Devuelve columnas (entre otras):
      id_student, id_playthrough, id_session, bdi_score,
      actions, apm, interactions, jumps, dashes, light_attacks, moves, inter_avg_s, uniq_types,
      social_interactions, aggression_score, exploration_score,
      decision_latency_avg, inactivity_periods, total_actions,
      start_time, end_time
    """
    sessions = get_playthroughs_with_logs(limit=limit)
    rows = []

    for s in sessions:
        df_log = clean_log_dataframe(s.get("log_data"), s.get("id_session"))
        feats = extract_behavior_features(df_log)  # ← genera las columnas avanzadas

        # Ignorar silenciosamente sesiones sin datos útiles
        if feats["actions"] == 0:
            continue

        row = {
            # Features avanzadas
            **feats,
            # Metadata
            "id_student": s.get("id_student"),
            "id_playthrough": s.get("id_playthrough"),
            "id_session": s.get("id_session"),
            "bdi_score": s.get("bdi_score"),
            # Vista (si están, las usamos; si no, van 0)
            "social_interactions": s.get("social_interactions"),
            "aggression_score": s.get("aggression_score"),
            "exploration_score": s.get("exploration_score"),
            "decision_latency_avg": s.get("decision_latency_avg"),
            "inactivity_periods": s.get("inactivity_periods"),
            "total_actions": s.get("total_actions"),
            "start_time": s.get("start_time"),
            "end_time": s.get("end_time"),
        }
        rows.append(row)

    if not rows:
        # No hay sesiones válidas
        return pd.DataFrame()

    ses_df = pd.DataFrame(rows)

    # Rellenar NaN de métricas opcionales con 0
    fill_zero = [
        "social_interactions", "aggression_score", "exploration_score",
        "decision_latency_avg", "inactivity_periods", "total_actions", "bdi_score"
    ]
    for c in fill_zero:
        if c in ses_df.columns:
            ses_df[c] = pd.to_numeric(ses_df[c], errors="coerce").fillna(0)

    # Asegurar numéricos en features avanzadas
    numeric_like = [
        "actions", "apm", "interactions", "jumps", "dashes", "light_attacks", "moves",
        "inter_avg_s", "uniq_types",
        "social_interactions", "aggression_score", "exploration_score",
        "decision_latency_avg", "inactivity_periods", "total_actions",
        "bdi_score"
    ]
    for c in numeric_like:
        if c in ses_df.columns:
            ses_df[c] = pd.to_numeric(ses_df[c], errors="coerce").fillna(0)

    return ses_df

# =========================
# Agregaciones por jugador
# =========================
def _aggregate_by_player_mean(ses_df: pd.DataFrame) -> pd.DataFrame:
    """Promedio por jugador de todas sus sesiones."""
    agg_cols = [
        "actions", "apm", "interactions", "jumps", "dashes", "light_attacks", "moves",
        "inter_avg_s", "uniq_types",
        "social_interactions", "aggression_score", "exploration_score",
        "decision_latency_avg", "inactivity_periods", "total_actions",
        "bdi_score"
    ]
    grp = ses_df.groupby("id_student", as_index=False)[agg_cols].mean()
    grp["total_sessions"] = ses_df.groupby("id_student")["id_session"].nunique().values
    return grp

def _aggregate_by_player_sum(ses_df: pd.DataFrame) -> pd.DataFrame:
    """Suma total (intensidad) en la mayoría de métricas y promedio en otras."""
    sum_cols = [
        "actions", "interactions", "jumps", "dashes", "light_attacks", "moves",
        "social_interactions", "aggression_score", "exploration_score",
        "inactivity_periods", "total_actions"
    ]
    mean_cols = ["apm", "inter_avg_s", "uniq_types", "decision_latency_avg", "bdi_score"]

    sum_df = ses_df.groupby("id_student", as_index=False)[sum_cols].sum()
    mean_df = ses_df.groupby("id_student", as_index=False)[mean_cols].mean()

    grp = pd.merge(sum_df, mean_df, on="id_student", how="inner")
    grp["total_sessions"] = ses_df.groupby("id_student")["id_session"].nunique().values
    return grp

def _aggregate_by_player_latest(ses_df: pd.DataFrame) -> pd.DataFrame:
    """Toma la última sesión del jugador (end_time desc, luego start_time desc)."""
    ts = ses_df.copy()
    for col in ["end_time", "start_time"]:
        if col in ts.columns:
            ts[col] = pd.to_datetime(ts[col], errors="coerce")
    ts = ts.sort_values(by=["id_student", "end_time", "start_time"], ascending=[True, False, False])
    latest = ts.groupby("id_student", as_index=False).head(1).copy()

    keep_cols = [
        "id_student",
        "actions", "apm", "interactions", "jumps", "dashes", "light_attacks", "moves",
        "inter_avg_s", "uniq_types",
        "social_interactions", "aggression_score", "exploration_score",
        "decision_latency_avg", "inactivity_periods", "total_actions",
        "bdi_score"
    ]
    for c in keep_cols:
        if c not in latest.columns:
            latest[c] = 0
    latest = latest[keep_cols].reset_index(drop=True)
    latest["total_sessions"] = ts.groupby("id_student")["id_session"].nunique().reindex(latest["id_student"]).values
    return latest

# =========================
# K-Means
# =========================
def _run_kmeans_players(players_df: pd.DataFrame, k=4):
    """
    Corre K-Means con StandardScaler; devuelve:
      clustered_df, summary_df, silhouette, km_model, scaler, feat_cols, players_features_df
    """
    # Requerimos al menos k+1 jugadores
    if players_df is None or players_df.empty or len(players_df) <= k:
        raise ValueError(f"No hay suficientes datos para clusterizar jugadores (players={0 if players_df is None else len(players_df)}, required>{k}).")

    feat_cols = [
        "actions", "apm", "interactions", "jumps", "dashes", "light_attacks", "moves",
        "inter_avg_s", "uniq_types",
        "social_interactions", "aggression_score", "exploration_score",
        "decision_latency_avg", "inactivity_periods", "total_actions",
        "bdi_score"
    ]
    for c in feat_cols:
        if c not in players_df.columns:
            players_df[c] = 0

    X = players_df[feat_cols].copy()
    scaler = StandardScaler()
    Xs = scaler.fit_transform(X)

    km = KMeans(n_clusters=k, random_state=42, n_init="auto")
    labels = km.fit_predict(Xs)

    clustered = players_df.copy()
    clustered["cluster"] = labels

    sil = -1.0
    try:
        if len(set(labels)) > 1 and len(players_df) > k:
            sil = float(silhouette_score(Xs, labels))
    except Exception:
        sil = -1.0

    summary_cols = feat_cols + ["total_sessions"]
    summary = clustered.groupby("cluster")[summary_cols].mean().reset_index()
    summary["n_players"] = clustered.groupby("cluster")["id_student"].nunique().values

    return clustered, summary, sil, km, scaler, feat_cols, players_df

# =========================
# API pública
# =========================
def kmeans_players_mean(k=4, limit=10000):
    ses_df = _build_session_features(limit=limit)
    if ses_df is None or ses_df.empty:
        raise ValueError("No hay sesiones con logs válidos para construir features por jugador.")
    players_df = _aggregate_by_player_mean(ses_df)
    clustered, summary, sil, km, scaler, feat_cols, players_feat = _run_kmeans_players(players_df, k=k)
    return clustered, summary, sil, km, scaler, feat_cols, players_feat


def kmeans_players_sum(k=4, limit=10000):
    ses_df = _build_session_features(limit=limit)
    if ses_df is None or ses_df.empty:
        raise ValueError("No hay sesiones con logs válidos para construir features por jugador.")
    players_df = _aggregate_by_player_sum(ses_df)
    clustered, summary, sil, km, scaler, feat_cols, players_feat = _run_kmeans_players(players_df, k=k)
    return clustered, summary, sil, km, scaler, feat_cols, players_feat


def kmeans_players_latest(k=4, limit=10000):
    ses_df = _build_session_features(limit=limit)
    if ses_df is None or ses_df.empty:
        raise ValueError("No hay sesiones con logs válidos para construir features por jugador.")
    players_df = _aggregate_by_player_latest(ses_df)
    clustered, summary, sil, km, scaler, feat_cols, players_feat = _run_kmeans_players(players_df, k=k)
    return clustered, summary, sil, km, scaler, feat_cols, players_feat
