from config.firestore_config import get_firestore_client

db = get_firestore_client()

def get_log(log_id: str):
    doc_ref = db.collection("session_logs").document(log_id)
    doc = doc_ref.get()
    return doc.to_dict() if doc.exists else None

def get_all_logs():
    docs = db.collection("session_logs").stream()
    return [{ "id": doc.id, **doc.to_dict() } for doc in docs]

def save_log(log_id: str, data: dict):
    db.collection("session_logs").document(log_id).set(data)
    return f"✅ Log {log_id} guardado."

def update_log(log_id: str, data: dict):
    db.collection("session_logs").document(log_id).update(data)
    return f"🔄 Log {log_id} actualizado."
