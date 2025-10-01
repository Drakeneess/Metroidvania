import firebase_admin
from firebase_admin import credentials, firestore
import os

def get_firestore_client():
    """
    Inicializa y devuelve el cliente de Firestore.
    """
    if not firebase_admin._apps:  # evita inicialización doble
        cred_path = os.path.join(os.path.dirname(__file__), "..", "secrets.json")
        cred = credentials.Certificate(cred_path)
        firebase_admin.initialize_app(cred)

    return firestore.client()
