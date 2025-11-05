from flask import Blueprint, jsonify, request
import pandas as pd
import os

# Para auto-generar clustering si falta
from analysis.kmeans_players import (
    kmeans_players_mean,
    kmeans_players_sum,
    kmeans_players_latest
)

player_bp = Blueprint("player", __name__)

DEFAULT_MODE = "mean"
VALID_MODES = {"mean", "sum", "latest"}

def _mode_from_query() -> str:
    mode = (request.args.get("mode") or DEFAULT_MODE).strip().lower()
    return mode if mode in VALID_MODES else DEFAULT_MODE

def _filenames_for_mode(mode: str):
    clusters = f"clusters_por_jugador_{mode}.csv"
    feats = f"player_features_{mode}.csv"
    return clusters, feats

def _ensure_clusters(mode: str):
    """
    Verifica si existen los CSV para el modo dado; si no existen,
    ejecuta el clustering correspondiente y los genera.
    Devuelve (auto_generated: bool, info_message: str or None)
    """
    clusters, feats = _filenames_for_mode(mode)
    have_clusters = os.path.exists(clusters)
    have_feats = os.path.exists(feats)

    if have_clusters and have_feats:
        return False, None  # ya existen

    # Generar automáticamente según el modo
    try:
        if mode == "mean":
            clustered, summary, sil, *_rest = kmeans_players_mean(k=4, limit=10000)
            players_features_df = _rest[-1]
            clustered[["id_student", "bdi_score", "total_sessions", "cluster"]].to_csv(clusters, index=False)
            players_features_df.to_csv(feats, index=False)
        elif mode == "sum":
            clustered, summary, sil, *_rest = kmeans_players_sum(k=4, limit=10000)
            players_features_df = _rest[-1]
            clustered[["id_student", "bdi_score", "total_sessions", "cluster"]].to_csv(clusters, index=False)
            players_features_df.to_csv(feats, index=False)
        elif mode == "latest":
            clustered, summary, sil, *_rest = kmeans_players_latest(k=4, limit=10000)
            players_features_df = _rest[-1]
            clustered[["id_student", "bdi_score", "total_sessions", "cluster"]].to_csv(clusters, index=False)
            players_features_df.to_csv(feats, index=False)
        return True, "El clustering no existía y fue generado automáticamente."
    except ValueError as ve:
        # Insuficientes jugadores o sin sesiones válidas
        return False, f"No fue posible generar el clustering automáticamente: {str(ve)}"
    except Exception as e:
        return False, f"Error generando clustering automáticamente: {str(e)}"

def _load_csv_safe(path: str):
    if not os.path.exists(path):
        return None
    try:
        return pd.read_csv(path)
    except Exception:
        return None

# ---------------- Endpoints ----------------

@player_bp.route("/player_cluster/<int:id_student>", methods=["GET"])
def player_cluster(id_student: int):
    mode = _mode_from_query()
    clusters_path, feats_path = _filenames_for_mode(mode)

    auto_generated, info = _ensure_clusters(mode)
    if info and not auto_generated:
        # hubo un problema al autogenerar
        return jsonify({"error": info, "mode": mode}), 400

    df_clusters = _load_csv_safe(clusters_path)
    if df_clusters is None or df_clusters.empty:
        return jsonify({"error": f"No se pudo cargar {clusters_path}. Ejecuta /analyze_players primero."}), 400

    row = df_clusters.loc[df_clusters["id_student"] == id_student]
    if row.empty:
        return jsonify({"message": "Jugador no encontrado en clusters", "id_student": id_student, "mode": mode}), 404

    res = {
        "mode": mode,
        "id_student": int(row.iloc[0]["id_student"]),
        "cluster": int(row.iloc[0]["cluster"]),
        "bdi_score": float(row.iloc[0]["bdi_score"]),
        "total_sessions": int(row.iloc[0]["total_sessions"])
    }
    if auto_generated:
        res["auto_generated"] = True
        res["info"] = info
    return jsonify(res), 200


@player_bp.route("/player_features/<int:id_student>", methods=["GET"])
def player_features(id_student: int):
    mode = _mode_from_query()
    clusters_path, feats_path = _filenames_for_mode(mode)

    auto_generated, info = _ensure_clusters(mode)
    if info and not auto_generated:
        return jsonify({"error": info, "mode": mode}), 400

    df_feats = _load_csv_safe(feats_path)
    if df_feats is None or df_feats.empty:
        return jsonify({"error": f"No se pudo cargar {feats_path}. Ejecuta /analyze_players primero."}), 400

    row = df_feats.loc[df_feats["id_student"] == id_student]
    if row.empty:
        return jsonify({"message": "Jugador no encontrado en features", "id_student": id_student, "mode": mode}), 404

    # Devuelve todas las columnas disponibles como dict
    payload = row.iloc[0].to_dict()
    # Asegurar tipos básicos
    if "id_student" in payload:
        payload["id_student"] = int(payload["id_student"])

    res = {"mode": mode, "features": payload}
    if auto_generated:
        res["auto_generated"] = True
        res["info"] = info
    return jsonify(res), 200


@player_bp.route("/player_info/<int:id_student>", methods=["GET"])
def player_info(id_student: int):
    mode = _mode_from_query()
    clusters_path, feats_path = _filenames_for_mode(mode)

    auto_generated, info = _ensure_clusters(mode)
    if info and not auto_generated:
        return jsonify({"error": info, "mode": mode}), 400

    df_clusters = _load_csv_safe(clusters_path)
    df_feats = _load_csv_safe(feats_path)

    if df_clusters is None or df_clusters.empty or df_feats is None or df_feats.empty:
        return jsonify({"error": "No se pudieron cargar los resultados de clustering. Ejecuta /analyze_players primero.", "mode": mode}), 400

    row_c = df_clusters.loc[df_clusters["id_student"] == id_student]
    row_f = df_feats.loc[df_feats["id_student"] == id_student]

    if row_c.empty or row_f.empty:
        return jsonify({"message": "Jugador no encontrado", "id_student": id_student, "mode": mode}), 404

    basic = {
        "id_student": int(row_c.iloc[0]["id_student"]),
        "cluster": int(row_c.iloc[0]["cluster"]),
        "bdi_score": float(row_c.iloc[0]["bdi_score"]),
        "total_sessions": int(row_c.iloc[0]["total_sessions"]),
    }
    features = row_f.iloc[0].to_dict()
    if "id_student" in features:
        features["id_student"] = int(features["id_student"])

    res = {
        "mode": mode,
        "player": {**basic, "features": features}
    }
    if auto_generated:
        res["auto_generated"] = True
        res["info"] = info

    return jsonify(res), 200

@player_bp.route("/player_session_analysis/<int:id_student>", methods=["GET"])
def player_session_analysis(id_student: int):
    """
    Devuelve TODAS las sesiones del jugador con su cluster (por sesión) y métricas reales.
    Usa clusters_por_sesion.csv (que ahora incluye features limpios).
    Si mode=sum o latest → fallback a mode=mean con advertencia.
    """
    mode = _mode_from_query()
    fallback = False

    if mode in ["sum", "latest"]:
        fallback = True
        mode = "mean"

    csv_path = "clusters_por_sesion.csv"
    if not os.path.exists(csv_path):
        # Generar clustering de sesiones automáticamente
        try:
            from main import main_process
            main_process()
        except Exception as e:
            return jsonify({"error": f"No se pudo generar clustering por sesión: {str(e)}"}), 500

    try:
        df = pd.read_csv(csv_path)
    except Exception as e:
        return jsonify({"error": f"No se pudo cargar {csv_path}: {str(e)}"}), 500

    # Filtrar por jugador
    pdf = df[df["id_student"] == id_student].copy()
    if pdf.empty:
        return jsonify({
            "message": "El jugador no tiene sesiones registradas.",
            "id_student": id_student
        }), 404

    # Ordenar por start_time desc (si existe), si no por id_session desc
    if "start_time" in pdf.columns:
        try:
            pdf["start_time"] = pd.to_datetime(pdf["start_time"], errors="coerce")
            pdf = pdf.sort_values(by=["start_time", "id_session"], ascending=[False, False])
        except Exception:
            pdf = pdf.sort_values(by="id_session", ascending=False)
    else:
        pdf = pdf.sort_values(by="id_session", ascending=False)

    # Campos a exponer por sesión
    cols = [
        "id_session", "cluster",
        "actions", "apm", "jumps", "dashes", "light_attacks",
        "interactions", "uniq_types", "inter_avg_s", "movement", "stopped", "health"
    ]
    for c in cols:
        if c not in pdf.columns:
            pdf[c] = 0

    sessions_list = []
    for _, row in pdf.iterrows():
        sessions_list.append({
            "id_session": int(row["id_session"]),
            "startTime": (
                str(row["start_time"]) if "start_time" in pdf.columns and pd.notna(row["start_time"]) else None
            ),
            "cluster": int(row["cluster"]),
            "actions": float(row["actions"]),
            "apm": float(row["apm"]),
            "jumps": float(row["jumps"]),
            "dashes": float(row["dashes"]),
            "light_attacks": float(row["light_attacks"]),
            "interactions": float(row["interactions"]),
            "uniq_types": float(row["uniq_types"]),
            "inter_avg_s": float(row["inter_avg_s"]),
            "movement": float(row["movement"]),
            "stopped": float(row["stopped"]),
            "health": float(row["health"]),
        })

    response = {
        "id_student": id_student,
        "mode": mode,
        "total_sessions": len(sessions_list),
        "sessions": sessions_list
    }
    if fallback:
        response["warning"] = "El clustering por sesión solo está disponible en mode=mean, se usó fallback."

    return jsonify(response), 200
