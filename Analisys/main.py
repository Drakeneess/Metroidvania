import pandas as pd
from modules.session_logs import get_playthroughs_with_logs
from clean.cleaner import clean_log_dataframe
from analysis.kmeans_actions import kmeans_actions
from dotenv import load_dotenv

load_dotenv()

def main_process():
    print("=== K-Means v1 (acciones vs BDI) ===")
    sessions = get_playthroughs_with_logs(limit=10000)

    # 1) Construir DF limpio
    dfs = []
    for s in sessions:
        df = clean_log_dataframe(s.get("log_data"), s["id_session"])
        if not df.empty:
            dfs.append(df)

    if not dfs:
        raise SystemExit("⚠ No hay acciones limpias para clusterizar.")

    final_df = pd.concat(dfs, ignore_index=True)

    # 2) K-Means (k=3) → devuelve DF con nombres LIMPIOS y cluster
    clustered, summary, sil, km, scaler, feat_cols = kmeans_actions(final_df, sessions, k=3)

    print("\n— Resumen por clúster —")
    print(summary.to_string(index=False))
    print(f"\nSilhouette score: {sil:.3f}")

    # 3) Guardar resultados (SIEMPRE sobrescribe)
    # Orden exacto aprobado:
    export_order = [
        "id_session",
        "id_playthrough",
        "id_student",
        "start_time",
        "bdi_score",
        "cluster",
        "actions",
        "apm",
        "jumps",
        "dashes",
        "light_attacks",
        "interactions",
        "uniq_types",
        "inter_avg_s",
        "movement",
        "stopped",
        "health",
    ]
    # asegurar columnas por si falta alguna
    for c in export_order:
        if c not in clustered.columns:
            clustered[c] = 0

    clustered[export_order].to_csv("clusters_por_sesion.csv", index=False)
    print("\n💾 clusters_por_sesion.csv (features + cluster) regenerado.")

    return summary, sil


if __name__ == "__main__":
    main_process()
