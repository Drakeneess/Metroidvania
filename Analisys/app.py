from flask import Flask, jsonify
from flask_cors import CORS
import os

# Blueprints
from api.clustering_routes import clustering_bp
from api.player_routes import player_bp

def create_app():
    app = Flask(__name__)

    # ✅ Habilitar CORS para desarrollo y Vercel
    CORS(app, resources={r"/*": {
        "origins": [
            "http://localhost:5173",
            "https://shadowofsoulspanel-l187espyo-drakeneess-projects.vercel.app",
            "https://shadowofsoulspanel-3h9zih21k-drakeneess-projects.vercel.app"
        ],
        "supports_credentials": True
    }})

    @app.route("/")
    def home():
        return jsonify({
            "status": "SoS Behavior Analytics Running 🚀",
            "message": "Endpoints: /analyze, /analyze_players, /analyze_players_sum, /analyze_players_latest, /player_cluster/<id>, /player_features/<id>, /player_info/<id>"
        })

    # Registrar Blueprints
    app.register_blueprint(clustering_bp)
    app.register_blueprint(player_bp)

    return app

app = create_app()

if __name__ == "__main__":
    port = int(os.getenv("PORT", 5000))
    app.run(host="0.0.0.0", port=port)
