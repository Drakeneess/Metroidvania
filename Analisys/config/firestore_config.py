import firebase_admin
from firebase_admin import credentials, firestore
import os
import base64
import json

def get_firestore_client():
    """
    Inicializa y devuelve el cliente de Firestore usando variables de entorno.
    """
    if not firebase_admin._apps:
        # 🔹 Si usas Railway o variables de entorno, se recomienda guardar la key en base64
        cred_base64 = os.getenv("FIREBASE_CREDENTIALS_BASE64")

        if cred_base64:
            # decodificar y cargar desde variable
            cred_dict = json.loads(base64.b64decode(cred_base64).decode("utf-8"))
            cred = credentials.Certificate(cred_dict)
        else:
            # 🔹 fallback local (usa secrets.json)
            cred_path = os.path.join(os.path.dirname(__file__), "..", "secrets.json")
            cred = credentials.Certificate(cred_path)

        firebase_admin.initialize_app(cred)

    return firestore.client()
