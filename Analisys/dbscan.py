from modules.session_logs import get_playthroughs_with_logs
from clean.cleaner import clean_log_dataframe
import pandas as pd

from analysis.dbscan_actions import dbscan_actions

print("=== DBSCAN v1 (acciones vs BDI) ===")
sessions = get_playthroughs_with_logs(limit=200)

# 1) Construir DF de acciones limpio
dfs = []
for s in sessions:
    df = clean_log_dataframe(s.get("log_data"), s["id_session"])
    if not df.empty:
        dfs.append(df)

if not dfs:
    raise SystemExit("⚠ No hay acciones limpias para clusterizar.")

final_df = pd.concat(dfs, ignore_index=True)

# 2) DBSCAN
clustered, summary, dbs, scaler, feat_cols = dbscan_actions(final_df, sessions, eps=1.0, min_samples=3)

print("\n— Resumen por clúster —")
print(summary.to_string(index=False))

# 3) Guardar resultados
clustered[["id_session","id_playthrough","id_student","bdi_score","cluster"]].to_csv("dbscan_clusters.csv", index=False)
print("\n💾 dbscan_clusters.csv generado.")
