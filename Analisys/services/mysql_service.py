from config.mysql_config import get_mysql_connection

def get_playthrough_summary(limit=5):
    """
    Obtiene los primeros 'limit' registros de la vista vw_playthrough_summary.
    """
    conn = get_mysql_connection()
    if conn is None:
        return []

    cursor = conn.cursor(dictionary=True)  # resultados como dict
    cursor.execute("SELECT * FROM vw_playthrough_summary LIMIT %s", (limit,))
    results = cursor.fetchall()

    cursor.close()
    conn.close()
    return results
