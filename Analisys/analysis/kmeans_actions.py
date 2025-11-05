# analysis/kmeans_actions.py
import pandas as pd
from sklearn.preprocessing import StandardScaler
from sklearn.cluster import KMeans
from sklearn.metrics import silhouette_score

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

_CLEAN_RENAME_MAP = {
    "total_actions": "actions",
    "actions_per_minute": "apm",
    "Jump": "jumps",
    "Dash": "dashes",
    "Light Attacking": "light_attacks",
    "Movement": "movement",
    "Stopped": "stopped",
    "interactions_count": "interactions",
    "interaction_avg_s": "inter_avg_s",
    "unique_action_types": "uniq_types",
    "mean_health": "health",
}

def kmeans_actions(final_actions_df: pd.DataFrame,
                   sessions_rows: list,
                   k: int = 4,
                   feature_cols = None):
    """
    final_actions_df: DF limpio de acciones (todas las sesiones) con columnas: id_session, type, timestamp, currentHealth...
    sessions_rows: lista de dicts (salida de get_playthroughs_with_logs) con bdi_score/start_time por id_session.
    """
    if feature_cols is None:
        feature_cols = FEATURE_COLS_DEFAULT

    # 1) Construir features por sesión
    from analysis.features import build_session_features
    feats = build_session_features(final_actions_df)
    if feats.empty:
        raise ValueError("No hay acciones para construir features.")

    # 2) Traer BDI + metadata por sesión desde la vista
    sess_df = pd.DataFrame(sessions_rows)
    cols = [c for c in [
        "id_session", "bdi_score", "id_level", "id_playthrough", "id_student", "start_time"
    ] if c in sess_df.columns]
    sess_df = sess_df[cols].drop_duplicates(subset=["id_session"])

    df = feats.merge(sess_df, on="id_session", how="left")
    df = df.dropna(subset=["bdi_score"])  # sesiones con BDI

    # 3) Escalar y clusterizar (solo features de acciones)
    X = df[feature_cols].astype(float).values
    scaler = StandardScaler()
    Xs = scaler.fit_transform(X)

    km = KMeans(n_clusters=k, n_init=10, random_state=42)
    labels = km.fit_predict(Xs)
    df["cluster"] = labels

    sil = silhouette_score(Xs, labels) if len(set(labels)) > 1 else float("nan")

    # 4) Resumen por clúster (mantiene tus etiquetas limpias)
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

    # 5) Devolver DF de sesiones con nombres LIMPIOS + metadata clave
    #    (esto es lo que guardaremos a disco desde main.py)
    export_cols = [
        "id_session", "id_playthrough", "id_student", "start_time", "bdi_score", "cluster",
        *feature_cols
    ]
    for c in export_cols:
        if c not in df.columns:
            df[c] = 0  # por si faltara algo

    clustered_clean = df[export_cols].rename(columns=_CLEAN_RENAME_MAP)

    return clustered_clean, summary, sil, km, scaler, list(_CLEAN_RENAME_MAP.values())
