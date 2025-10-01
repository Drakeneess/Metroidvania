// src/models/BdiItem.js
import { DataTypes } from "sequelize";
import { sequelize } from "../sequelize.js";

const BdiItem = sequelize.define("BdiItem", {
  id_item: {
    type: DataTypes.INTEGER,
    primaryKey: true,
    autoIncrement: true
  },
  item_number: {
    type: DataTypes.INTEGER,
    allowNull: false
  },
  title: {
    // En tu dataset los enunciados pueden ser extensos → TEXT
    type: DataTypes.TEXT,
    allowNull: false
  }
}, {
  tableName: "bdi_item",
  timestamps: false,
  indexes: [
    { fields: ["item_number"] } // útil para ordenar/consultar rápido
  ]
});

export default BdiItem;
