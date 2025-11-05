// src/sequelize.js
import { Sequelize } from "sequelize";
import dotenv from "dotenv";

dotenv.config();

export const sequelize = new Sequelize(
  process.env.DB_NAME,
  process.env.DB_USER,
  process.env.DB_PASSWORD,
  {
    host: process.env.DB_HOST || "localhost",
    dialect: "mysql",
    port: process.env.DB_PORT || 3306,
    logging: false,

    // ✅ Pool recomendado para Railway + Hostinger
    pool: {
      max: 10,         // máximo de conexiones
      min: 0,          // mínimo
      acquire: 30000,  // tiempo máximo para obtener conexión antes de lanzar error
      idle: 10000,     // tiempo para liberar conexión inactiva
    },

    dialectOptions: {
      connectTimeout: 20000, // ⏳ evita fallos al despertar el servicio
      // ssl: { require: true }  // 🔒 activar solo si Hostinger lo exige
    },
  }
);

// Probar conexión (lo dejamos, pero puedes quitarlo en producción)
try {
  await sequelize.authenticate();
  console.log("✅ Conexión a MySQL establecida con éxito");
} catch (error) {
  console.error("❌ Error al conectar a MySQL:", error);
}
