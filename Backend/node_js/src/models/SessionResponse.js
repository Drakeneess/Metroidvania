// src/models/SessionResponse.js
import { DataTypes } from "sequelize";
import { sequelize } from "../sequelize.js";
import Student from "./Student.js";
import BdiItem from "./BdiItem.js";
import BdiItemResponse from "./BdiItemResponse.js";

const SessionResponse = sequelize.define("SessionResponse", {
  id_session_response: {
    type: DataTypes.INTEGER,
    primaryKey: true,
    autoIncrement: true
  },
  id_student: {
    type: DataTypes.INTEGER,
    allowNull: false
  },
  id_item: {
    type: DataTypes.INTEGER,
    allowNull: false
  },
  id_response: {
    type: DataTypes.INTEGER,
    allowNull: false
  }
}, {
  tableName: "session_response",
  timestamps: false
});

// Relaciones
SessionResponse.belongsTo(Student, { foreignKey: "id_student" });
SessionResponse.belongsTo(BdiItem, { foreignKey: "id_item" });
SessionResponse.belongsTo(BdiItemResponse, { foreignKey: "id_response" });

export default SessionResponse;
