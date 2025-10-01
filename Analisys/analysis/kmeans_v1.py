import pandas as pd
from sklearn.cluster import KMeans
from sklearn.preprocessing import StandardScaler
import matplotlib.pyplot as plt

def prepare_features(actions_df, playthroughs_summary):
    """
    Une acciones limpias con BDI score y devuelve un DataFrame listo para clustering.
    """
    # Contar acciones por tipo
    counts = actions_df.groupby(["id_playthrough", "type"]).size().unstack(fill_value=0)

    # Añadir BDI score
    summary_df = pd.DataFrame(playthroughs_summary)
    merged = counts.merge(
        summary_df[["id_playthrough", "bdi_score"]],
        on="id_playthrough",
        how="left"
    ).fillna(0)

    return merged

def run_kmeans(features_df, k=3):
    """
    Aplica KMeans y devuelve etiquetas.
    """
    scaler = StandardScaler()
    X_scaled = scaler.fit_transform(features_df.drop(columns=["bdi_score"]))

    kmeans = KMeans(n_clusters=k, random_state=42)
    features_df["cluster"] = kmeans.fit_predict(X_scaled)

    return features_df, kmeans

def plot_clusters(features_df):
    """
    Visualiza clusters usando las dos primeras features.
    """
    plt.figure(figsize=(8,6))
    scatter = plt.scatter(
        features_df.iloc[:,0], 
        features_df.iloc[:,1], 
        c=features_df["cluster"], 
        cmap="viridis"
    )
    plt.xlabel(features_df.columns[0])
    plt.ylabel(features_df.columns[1])
    plt.title("Clusters de jugadores (acciones vs BDI)")
    plt.colorbar(scatter, label="Cluster")
    plt.show()
