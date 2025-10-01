# analysis/features.py
import pandas as pd
import numpy as np

EXPECTED_TYPES = [
    "Jump", "Dash", "Light Attacking", "Movement", "Stopped",
    "BeginInteraction", "EndInteraction"
]

def _pair_interactions(session_df: pd.DataFrame):
    """Empareja BeginInteraction→EndInteraction en orden y devuelve duraciones (s)."""
    starts = []
    durations = []
    for _, row in session_df.sort_values("timestamp").iterrows():
        t = row["timestamp"]
        typ = row["type"]
        if typ == "BeginInteraction":
            starts.append(t)
        elif typ == "EndInteraction" and starts:
            start = starts.pop(0)  # FIFO
            durations.append((t - start).total_seconds())
    return durations

def build_session_features(actions_df: pd.DataFrame) -> pd.DataFrame:
    """
    Recibe el DF limpio de acciones (todas las sesiones) y devuelve un DF
    con una fila por sesión y columnas de features numéricos.
    """
    if actions_df.empty:
        return pd.DataFrame()

    feats = []
    for id_session, g in actions_df.groupby("id_session"):
        g = g.sort_values("timestamp")
        total_actions = len(g)

        # conteos por tipo
        counts = g["type"].value_counts().reindex(EXPECTED_TYPES, fill_value=0)

        # duración de la sesión (min)
        tmin = g["timestamp"].min()
        tmax = g["timestamp"].max()
        dur_min = max((tmax - tmin).total_seconds() / 60.0, 1e-6)  # evitar /0

        # acciones por minuto
        apm = total_actions / dur_min

        # interacciones
        inter_durs = _pair_interactions(g)
        inter_count = len(inter_durs)
        inter_avg_s = float(np.mean(inter_durs)) if inter_durs else 0.0

        # diversidad de acciones
        unique_types = g["type"].nunique()

        # salud promedio (opcional)
        mean_health = float(g["currentHealth"].mean()) if "currentHealth" in g.columns else 0.0

        row = {
            "id_session": id_session,
            "total_actions": int(total_actions),
            "actions_per_minute": float(apm),
            "interaction_avg_s": float(inter_avg_s),
            "interactions_count": int(inter_count),
            "unique_action_types": int(unique_types),
            "mean_health": mean_health,
        }
        # Agrega las columnas por tipo
        for typ in EXPECTED_TYPES:
            row[typ] = int(counts.get(typ, 0))

        feats.append(row)

    return pd.DataFrame(feats)
