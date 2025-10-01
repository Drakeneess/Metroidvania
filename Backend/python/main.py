import os
import json
import datetime
import uuid
from fastapi import FastAPI, Request
from google.cloud import firestore
from google.oauth2 import service_account

app = FastAPI()

# 🔹 Inicializar Firestore
def init_firestore():
    creds = None
    project_id = None

    if "GOOGLE_APPLICATION_CREDENTIALS_JSON" in os.environ:
        key_dict = json.loads(os.environ["GOOGLE_APPLICATION_CREDENTIALS_JSON"])
        creds = service_account.Credentials.from_service_account_info(key_dict)
        project_id = key_dict["shadowofsouls-f4b5f"]
    elif os.path.exists("secrets.json"):
        creds = service_account.Credentials.from_service_account_file("secrets.json")
        with open("secrets.json") as f:
            project_id = json.load(f)["project_id"]
    else:
        raise Exception("❌ No se encontraron credenciales de Firebase")

    return firestore.Client(credentials=creds, project=project_id)

db = init_firestore()

# 🔹 Endpoint para recibir logs
@app.post("/upload")
async def upload_log(request: Request):
    data = await request.json()
    id_session = data.get("id_session")
    log_data   = data.get("log_data")

    if not id_session or not log_data:
        return {"success": False, "error": "Faltan parámetros"}

    # Generar un ID único para Firestore
    nosql_log_id = str(uuid.uuid4())

    # Guardar en Firestore
    db.collection("session_logs").document(nosql_log_id).set({
        "id_session": id_session,
        "created_at": datetime.datetime.utcnow().isoformat(),
        "log_data": log_data
    })

    return {"success": True, "id": nosql_log_id}

# 🔹 Health check
@app.get("/")
def root():
    return {"status": "ok", "message": "🔥 Python Log Service activo"}
