from modules.session_logs import get_playthroughs_with_logs
from clean.cleaner import clean_log_dataframe
import pandas as pd

from analysis.kmeans_actions import kmeans_actions

print("=== K-Means v1 (acciones vs BDI) ===")
sessions = get_playthroughs_with_logs(limit=200)

# 1) Construir DF de acciones limpio (todas las sesiones)
dfs = []
for s in sessions:
    df = clean_log_dataframe(s.get("log_data"), s["id_session"])
    if not df.empty:
        dfs.append(df)

if not dfs:
    raise SystemExit("⚠ No hay acciones limpias para clusterizar.")

final_df = pd.concat(dfs, ignore_index=True)

# 2) K-Means (k=3 por defecto)
clustered, summary, sil, km, scaler, feat_cols = kmeans_actions(final_df, sessions, k=3)

print("\n— Resumen por clúster —")
print(summary.to_string(index=False))
print(f"\nSilhouette score: {sil:.3f}")

# 3) (Opcional) Guardar resultados
clustered[["id_session","id_playthrough","id_student","bdi_score","cluster"]].to_csv("clusters_por_sesion.csv", index=False)
print("\n💾 clusters_por_sesion.csv generado.")
