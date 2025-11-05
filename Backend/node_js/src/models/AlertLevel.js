import { DataTypes, Model } from "sequelize";
import { sequelize } from "../sequelize.js";

class AlertLevel extends Model {}

AlertLevel.init(
  {
    id_alert: { type: DataTypes.INTEGER, primaryKey: true, autoIncrement: true },
    alert_type: { type: DataTypes.STRING(50), allowNull: false },
    alert_color: { type: DataTypes.STRING(20), allowNull: false },
    alert_priority: { type: DataTypes.INTEGER, allowNull: false, defaultValue: 0 },
  },
  { sequelize, tableName: "alert_level", timestamps: false }
);

export default AlertLevel;
