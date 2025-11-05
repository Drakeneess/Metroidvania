from flask import Blueprint, jsonify
from main import main_process
from analysis.kmeans_players import (
    kmeans_players_mean,
    kmeans_players_sum,
    kmeans_players_latest
)

clustering_bp = Blueprint("clustering", __name__)

def _ok(payload: dict):
    return jsonify(payload), 200

def _not_enough(players_count: int, required: int):
    return jsonify({
        "message": "No hay suficientes datos para clusterizar jugadores.",
        "players_detected": players_count,
        "required_minimum": required + 1
    }), 400

# --------- EXISTENTE (por sesión) ----------
@clustering_bp.route("/analyze", methods=["POST"])
def analyze_sessions():
    try:
        summary, sil = main_process()
        return _ok({
            "mode": "session",
            "message": "Clustering por sesión completado con éxito",
            "silhouette_score": round(sil, 4),
            "summary": summary.to_dict(orient="records")
        })
    except SystemExit as se:
        return jsonify({"message": str(se)}), 400
    except Exception as e:
        return jsonify({"error": str(e)}), 500

# --------- NUEVOS (por jugador) ----------
@clustering_bp.route("/analyze_players", methods=["POST"])
def analyze_players_mean():
    try:
        clustered, summary, sil, *_rest = kmeans_players_mean(k=4, limit=10000)
        players_features_df = _rest[-1]
        clustered[["id_student", "bdi_score", "total_sessions", "cluster"]].to_csv(
            "clusters_por_jugador_mean.csv", index=False
        )
        players_features_df.to_csv("player_features_mean.csv", index=False)

        return _ok({
            "mode": "player_mean",
            "message": "Clustering por jugador (PROMEDIO) completado",
            "silhouette_score": round(sil, 4),
            "summary": summary.to_dict(orient="records")
        })
    except ValueError as ve:
        msg = str(ve)
        players = 0
        if "players=" in msg:
            try:
                players = int(msg.split("players=")[1].split(",")[0].strip(") "))
            except Exception:
                players = 0
        return _not_enough(players, 4)
    except Exception as e:
        return jsonify({"error": str(e)}), 500


@clustering_bp.route("/analyze_players_sum", methods=["POST"])
def analyze_players_sum():
    try:
        clustered, summary, sil, *_rest = kmeans_players_sum(k=4, limit=10000)
        players_features_df = _rest[-1]
        clustered[["id_student", "bdi_score", "total_sessions", "cluster"]].to_csv(
            "clusters_por_jugador_sum.csv", index=False
        )
        players_features_df.to_csv("player_features_sum.csv", index=False)

        return _ok({
            "mode": "player_sum",
            "message": "Clustering por jugador (SUMA TOTAL) completado",
            "silhouette_score": round(sil, 4),
            "summary": summary.to_dict(orient="records")
        })
    except ValueError as ve:
        msg = str(ve)
        players = 0
        if "players=" in msg:
            try:
                players = int(msg.split("players=")[1].split(",")[0].strip(") "))
            except Exception:
                players = 0
        return _not_enough(players, 4)
    except Exception as e:
        return jsonify({"error": str(e)}), 500


@clustering_bp.route("/analyze_players_latest", methods=["POST"])
def analyze_players_latest():
    try:
        clustered, summary, sil, *_rest = kmeans_players_latest(k=4, limit=10000)
        players_features_df = _rest[-1]
        clustered[["id_student", "bdi_score", "total_sessions", "cluster"]].to_csv(
            "clusters_por_jugador_latest.csv", index=False
        )
        players_features_df.to_csv("player_features_latest.csv", index=False)

        return _ok({
            "mode": "player_latest",
            "message": "Clustering por jugador (ÚLTIMA SESIÓN) completado",
            "silhouette_score": round(sil, 4),
            "summary": summary.to_dict(orient="records")
        })
    except ValueError as ve:
        msg = str(ve)
        players = 0
        if "players=" in msg:
            try:
                players = int(msg.split("players=")[1].split(",")[0].strip(") "))
            except Exception:
                players = 0
        return _not_enough(players, 4)
    except Exception as e:
        return jsonify({"error": str(e)}), 500
