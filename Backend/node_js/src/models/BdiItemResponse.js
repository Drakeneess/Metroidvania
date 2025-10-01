// src/models/BdiItemResponse.js
import { DataTypes } from "sequelize";
import { sequelize } from "../sequelize.js";

const BdiItemResponse = sequelize.define("BdiItemResponse", {
  id_response: {
    type: DataTypes.INTEGER,
    primaryKey: true,
    autoIncrement: true
  },
  id_item: {
    type: DataTypes.INTEGER,
    allowNull: false
  },
  response: {
    // Texto “completo” (clínico)
    type: DataTypes.TEXT,
    allowNull: false
  },
  response_symbol: {
    // Versión corta/estilizada para tu narrativa (puede ser null)
    type: DataTypes.STRING,
    allowNull: true
  },
  score: {
    type: DataTypes.INTEGER,
    allowNull: false
  }
}, {
  tableName: "bdi_item_response",
  timestamps: false,
  indexes: [
    { fields: ["id_item"] }, // FK lookup
    { fields: ["score"] }    // para distribuciones y filtros
  ]
});

export default BdiItemResponse;
