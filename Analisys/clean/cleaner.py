import pandas as pd
import json

def clean_log_dataframe(raw_log: dict, id_session: int):
    """
    Limpia un log crudo de Firestore y devuelve un DataFrame listo para análisis.
    """
    if not raw_log or "log_data" not in raw_log:
        return pd.DataFrame()  # vacío si no hay log

    try:
        data = json.loads(raw_log["log_data"])
        actions = data.get("actions", [])
    except Exception as e:
        print(f"⚠ Error parseando log de sesión {id_session}: {e}")
        return pd.DataFrame()

    if not actions:
        return pd.DataFrame()

    df = pd.DataFrame(actions)

    # Asegurar columnas clave
    expected_cols = ["type", "actionName", "timestamp", "posX", "posY", "currentHealth", "extras"]
    for col in expected_cols:
        if col not in df.columns:
            df[col] = None

    # Convertir timestamp a datetime
    df["timestamp"] = pd.to_datetime(df["timestamp"], errors="coerce")
    df = df.dropna(subset=["timestamp"])

    # Ordenar por tiempo
    df = df.sort_values("timestamp").reset_index(drop=True)

    # 🔹 Convertir extras en JSON string antes de deduplicar
    df["_extras_str"] = df["extras"].apply(lambda x: json.dumps(x, sort_keys=True) if isinstance(x, list) else str(x))

    # Eliminar duplicados considerando todas las columnas excepto "extras"
    df = df.drop_duplicates(subset=["type", "actionName", "timestamp", "posX", "posY", "currentHealth", "_extras_str"])

    # Restaurar extras como lista
    df["extras"] = df["extras"].apply(lambda x: x if isinstance(x, list) else [])

    # Agregar columna de sesión
    df["id_session"] = id_session

    # Quitar la columna auxiliar
    df = df.drop(columns=["_extras_str"])

    return df
