import os
import json
import base64
import datetime
import uuid
from fastapi import FastAPI, Request, HTTPException
from google.cloud import firestore
from google.oauth2 import service_account

app = FastAPI()

db = None  # se inicializa en startup

def build_credentials_and_project():
    """
    Retorna (credentials, project_id) a partir de:
    - GOOGLE_APPLICATION_CREDENTIALS_JSON_B64 (preferido)
    - GOOGLE_APPLICATION_CREDENTIALS_JSON (JSON plano)
    - secrets.json (archivo local, solo dev)
    """
    key_dict = None

    if "GOOGLE_APPLICATION_CREDENTIALS_JSON_B64" in os.environ:
        decoded = base64.b64decode(os.environ["GOOGLE_APPLICATION_CREDENTIALS_JSON_B64"])
        key_dict = json.loads(decoded)
    elif "GOOGLE_APPLICATION_CREDENTIALS_JSON" in os.environ:
        key_dict = json.loads(os.environ["GOOGLE_APPLICATION_CREDENTIALS_JSON"])
    elif os.path.exists("secrets.json"):
        with open("secrets.json", "r", encoding="utf-8") as f:
            key_dict = json.load(f)
    else:
        raise RuntimeError("❌ No se encontraron credenciales de Firebase")

    if "project_id" not in key_dict:
        raise RuntimeError("❌ Credenciales inválidas: falta 'project_id'")

    creds = service_account.Credentials.from_service_account_info(key_dict)
    project_id = key_dict["project_id"]
    return creds, project_id

@app.on_event("startup")
def startup_firestore():
    global db
    creds, project_id = build_credentials_and_project()
    db = firestore.Client(credentials=creds, project=project_id)

@app.post("/upload")
async def upload_log(request: Request):
    if db is None:
        raise HTTPException(status_code=500, detail="DB no inicializada")

    try:
        data = await request.json()
    except Exception:
        raise HTTPException(status_code=400, detail="JSON inválido")

    id_session = data.get("id_session")
    log_data = data.get("log_data")

    if not id_session or log_data is None:
        raise HTTPException(status_code=422, detail="Faltan parámetros: id_session y log_data")

    # ID único para Firestore
    nosql_log_id = str(uuid.uuid4())

    # Guardar en Firestore
    db.collection("session_logs").document(nosql_log_id).set({
        "id_session": id_session,
        "created_at": datetime.datetime.utcnow().isoformat() + "Z",
        "log_data": log_data
    })

    return {"success": True, "id": nosql_log_id}

@app.get("/")
def root():
    return {"status": "ok", "message": "🔥 Python Log Service activo"}
