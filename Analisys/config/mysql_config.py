import os
import mysql.connector
from mysql.connector import Error

def get_mysql_connection():
    try:
        connection = mysql.connector.connect(
            host=os.getenv("DB_HOST", "localhost"),
            port=int(os.getenv("DB_PORT", 3306)),
            user=os.getenv("DB_USER", "root"),
            password=os.getenv("DB_PASS", ""),
            database=os.getenv("DB_NAME", "")
        )
        if connection.is_connected():
            print(f"✅ Conectado a MySQL ({os.getenv('DB_HOST')})")
            return connection
    except Error as e:
        print(f"❌ Error al conectar a MySQL: {e}")
        return None
