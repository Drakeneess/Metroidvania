import { DataTypes, Model } from "sequelize";
import { sequelize } from "../sequelize.js";
import AlertLevel from "./AlertLevel.js";
import Student from "./Student.js";

class AlertStudent extends Model {}

AlertStudent.init(
  {
    id_alert_student: { type: DataTypes.INTEGER, primaryKey: true, autoIncrement: true },
    id_alert: { type: DataTypes.INTEGER, allowNull: false },
    id_student: { type: DataTypes.INTEGER, allowNull: false },
  },
  { sequelize, tableName: "alert_student", timestamps: false }
);

// Asociaciones locales
AlertStudent.belongsTo(AlertLevel, {
  foreignKey: "id_alert",
  targetKey: "id_alert",
  as: "alertLevel", // ✅ alias único
});

AlertStudent.belongsTo(Student, {
  foreignKey: "id_student",
  targetKey: "id_student",
  as: "student",
});

export default AlertStudent;
