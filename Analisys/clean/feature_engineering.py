import pandas as pd
import numpy as np

def _safe_seconds(delta):
    try:
        return delta.total_seconds()
    except Exception:
        return np.nan

def extract_behavior_features(df_session: pd.DataFrame) -> dict:
    """
    Calcula features avanzadas de comportamiento a partir del DF de acciones por sesión.
    Devuelve:
      actions, apm, interactions, jumps, dashes, light_attacks, moves, inter_avg_s, uniq_types
    """
    if df_session is None or df_session.empty:
        return {
            "actions": 0,
            "apm": 0.0,
            "interactions": 0,
            "jumps": 0,
            "dashes": 0,
            "light_attacks": 0,
            "moves": 0,
            "inter_avg_s": 0.0,
            "uniq_types": 0
        }

    df = df_session.copy()
    if "type" not in df.columns:
        df["type"] = None
    if "timestamp" not in df.columns:
        df["timestamp"] = pd.NaT

    df["timestamp"] = pd.to_datetime(df["timestamp"], errors="coerce")
    df = df.dropna(subset=["timestamp"]).sort_values("timestamp").reset_index(drop=True)

    total_actions = len(df)
    if total_actions == 0:
        return {
            "actions": 0,
            "apm": 0.0,
            "interactions": 0,
            "jumps": 0,
            "dashes": 0,
            "light_attacks": 0,
            "moves": 0,
            "inter_avg_s": 0.0,
            "uniq_types": 0
        }

    duration_s = _safe_seconds(df["timestamp"].iloc[-1] - df["timestamp"].iloc[0])
    duration_min = max(duration_s / 60.0, 1e-9) if pd.notna(duration_s) else 1e-9
    apm = float(total_actions / duration_min)

    inter_avg_s = 0.0
    if total_actions >= 2:
        diffs = df["timestamp"].diff().dropna().apply(_safe_seconds)
        diffs = diffs.replace([np.inf, -np.inf], np.nan).dropna()
        inter_avg_s = float(np.mean(diffs)) if len(diffs) > 0 else 0.0

    t = df["type"].astype(str)

    light_attacks = int((t == "Light Attacking").sum())
    dashes = int((t == "Dash").sum())
    jumps = int((t == "Jump").sum())
    interactions = int(t.isin(["BeginInteraction", "EndInteraction"]).sum())
    moves = int((t == "Movement").sum())
    uniq_types = int(t.nunique())

    return {
        "actions": int(total_actions),
        "apm": float(apm),
        "interactions": int(interactions),
        "jumps": int(jumps),
        "dashes": int(dashes),
        "light_attacks": int(light_attacks),
        "moves": int(moves),
        "inter_avg_s": float(inter_avg_s),
        "uniq_types": int(uniq_types)
    }
