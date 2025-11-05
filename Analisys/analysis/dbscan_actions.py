# analysis/dbscan_actions.py
import pandas as pd
from sklearn.preprocessing import StandardScaler
from sklearn.cluster import DBSCAN

FEATURE_COLS_DEFAULT = [
    "total_actions",
    "actions_per_minute",
    "Jump",
    "Dash",
    "Light Attacking",
    "Movement",
    "Stopped",
    "interactions_count",
    "interaction_avg_s",
    "unique_action_types",
    "mean_health",
]

def dbscan_actions(final_actions_df: pd.DataFrame,
                   sessions_rows: list,
                   eps: float = 0.8,
                   min_samples: int = 3,
                   feature_cols = None):
    """
    final_actions_df: DF limpio de acciones (todas las sesiones).
    sessions_rows: lista de dicts (vista + logs) con bdi_score.
    eps: radio máximo de densidad (ajustar).
    min_samples: mínimo de puntos para formar un cluster.
    """
    if feature_cols is None:
        feature_cols = FEATURE_COLS_DEFAULT

    from analysis.features import build_session_features
    feats = build_session_features(final_actions_df)
    if feats.empty:
        raise ValueError("No hay acciones para construir features.")

    # 🔹 Merge con info de BDI
    sess_df = pd.DataFrame(sessions_rows)
    cols = [c for c in ["id_session", "bdi_score", "id_level", "id_playthrough", "id_student"] if c in sess_df.columns]
    sess_df = sess_df[cols].drop_duplicates(subset=["id_session"])
    df = feats.merge(sess_df, on="id_session", how="left")
    df = df.dropna(subset=["bdi_score"])

    # 🔹 Escalar features
    X = df[feature_cols].astype(float).values
    scaler = StandardScaler()
    Xs = scaler.fit_transform(X)

    # 🔹 DBSCAN
    dbs = DBSCAN(eps=eps, min_samples=min_samples)
    labels = dbs.fit_predict(Xs)
    df["cluster"] = labels  # -1 = ruido

    # 🔹 Resumen
    summary = df.groupby("cluster").agg(
        n=("id_session", "count"),
        bdi_mean=("bdi_score", "mean"),
        bdi_std=("bdi_score", "std"),
        actions=("total_actions", "mean"),
        apm=("actions_per_minute", "mean"),
        jumps=("Jump", "mean"),
        dashes=("Dash", "mean"),
        light_attacks=("Light Attacking", "mean"),
        interactions=("interactions_count", "mean"),
        inter_avg_s=("interaction_avg_s", "mean"),
        uniq_types=("unique_action_types", "mean"),
    ).reset_index().sort_values("cluster")

    return df, summary, dbs, scaler, feature_cols
