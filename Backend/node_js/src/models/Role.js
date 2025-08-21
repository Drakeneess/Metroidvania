import { DataTypes } from "sequelize";
import { sequelize } from "../sequelize.js";


const Role = sequelize.define("Role", {
  id_role: { type: DataTypes.INTEGER, primaryKey: true, autoIncrement: true },
  name: { type: DataTypes.STRING, allowNull: false }
}, {
  tableName: "role",
  timestamps: false
});

export default Role;
