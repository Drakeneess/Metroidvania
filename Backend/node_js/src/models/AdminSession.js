import { DataTypes } from "sequelize";
import { sequelize } from "../sequelize.js";

const AdminSession = sequelize.define("AdminSession", {
  session_id: {
    type: DataTypes.UUID,
    defaultValue: DataTypes.UUIDV4,
    primaryKey: true,
  },
  id_user: {
    type: DataTypes.INTEGER,
    allowNull: false,
  },
  started_at: { type: DataTypes.DATE, allowNull: false },
  last_seen_at: { type: DataTypes.DATE, allowNull: false },
  ended_at: { type: DataTypes.DATE, allowNull: true },
  duration_sec: { type: DataTypes.INTEGER, allowNull: true },
  ip_address: { type: DataTypes.STRING(100), allowNull: true },
  user_agent: { type: DataTypes.STRING(512), allowNull: true },
}, {
  tableName: "admin_sessions",
  timestamps: false,
});

export default AdminSession;
