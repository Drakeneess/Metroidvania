import mysql.connector
from mysql.connector import Error

def get_mysql_connection():
    try:
        connection = mysql.connector.connect(
            host="82.197.82.77",         # Host de Hostinger
            port=3306,                          # Puerto MySQL
            user="u512280201_drakeneess",       # Usuario
            password="Deploy157@",              # Contraseña
            database="u512280201_shadow_of_soul" # Nombre de la base
        )
        if connection.is_connected():
            print("✅ Conectado a MySQL en Hostinger")
            return connection
    except Error as e:
        print(f"❌ Error al conectar a MySQL: {e}")
        return None