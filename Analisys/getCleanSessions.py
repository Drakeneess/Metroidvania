from modules.session_logs import get_playthroughs_with_logs
from clean.cleaner import clean_log_dataframe
import pandas as pd

print("=== Sesiones con Logs Limpios ===")
sessions = get_playthroughs_with_logs(limit=100)

# Recorremos y limpiamos cada log
dfs = []
for s in sessions:
    df = clean_log_dataframe(s.get("log_data"), s["id_session"])
    if not df.empty:
        # Añadimos contexto extra al DataFrame
        df["id_playthrough"] = s.get("id_playthrough")
        df["id_student"] = s.get("id_student")
        dfs.append(df)

# Concatenamos todos en uno solo
if dfs:
    final_df = pd.concat(dfs, ignore_index=True)
    print(final_df.head())
    print(f"✅ Total acciones limpias: {len(final_df)}")
else:
    print("⚠ No se encontraron acciones limpias.")
