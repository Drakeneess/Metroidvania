from config.mysql_config import get_mysql_connection
from config.firestore_config import get_firestore_client

db = get_firestore_client()

def get_playthroughs_with_logs(limit=100):
    """
    Obtiene playthroughs desde la vista y sus logs asociados en Firestore,
    ordenados por nosql_log_id.
    """
    conn = get_mysql_connection()
    if conn is None:
        return []

    cursor = conn.cursor(dictionary=True)
    query = """
        SELECT * 
        FROM vw_playthrough_summary 
        WHERE nosql_log_id IS NOT NULL
        ORDER BY nosql_log_id IS NULL, nosql_log_id ASC 
        LIMIT %s
    """
    cursor.execute(query, (limit,))
    playthroughs = cursor.fetchall()
    cursor.close()
    conn.close()

    # 🔹 Enriquecer con logs
    enriched = []
    for pt in playthroughs:
        log_data = None
        if pt.get("nosql_log_id"):  # si existe ID en la vista
            doc_ref = db.collection("session_logs").document(pt["nosql_log_id"])
            doc = doc_ref.get()
            if doc.exists:
                log_data = doc.to_dict()

        enriched.append({
            **pt,
            "log_data": log_data
        })

    return enriched
