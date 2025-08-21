// src/sequelize.js
import { Sequelize } from "sequelize";
import dotenv from "dotenv";

dotenv.config();

export const sequelize = new Sequelize(
  process.env.DB_NAME,      // nombre de la BD
  process.env.DB_USER,      // usuario
  process.env.DB_PASSWORD,  // contraseña
  {
    host: process.env.DB_HOST || "localhost",
    dialect: "mysql",
    port: process.env.DB_PORT || 3306,
    logging: false, // poner en true si quieres ver las queries en consola
  }
);

// Probar conexión
try {
  await sequelize.authenticate();
  console.log("✅ Conexión a MySQL establecida con éxito");
} catch (error) {
  console.error("❌ Error al conectar a MySQL:", error);
}
