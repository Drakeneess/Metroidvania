import { DataTypes } from "sequelize";
import { sequelize } from "../sequelize.js";

const User = sequelize.define("User", {
  id_user: {
    type: DataTypes.INTEGER,
    primaryKey: true,
    autoIncrement: true
  },
  full_name: {
    type: DataTypes.STRING,
    allowNull: false
  },
  email: {
    type: DataTypes.STRING,
    allowNull: false,
    unique: true
  },
  password: {
    type: DataTypes.STRING,
    allowNull: false
  },
  register_date: {
    type: DataTypes.DATE,
    defaultValue: DataTypes.NOW
  },
  id_role: {
    type: DataTypes.INTEGER
  },
  id_state: {
    type: DataTypes.INTEGER
  }
}, {
  tableName: "user_account",   // 👈 aquí está la clave
  timestamps: false
});

export default User;
