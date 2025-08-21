import { DataTypes } from "sequelize";
import { sequelize } from "../sequelize.js";

const State = sequelize.define("State", {
  id_state: { type: DataTypes.INTEGER, primaryKey: true, autoIncrement: true },
  name: { type: DataTypes.STRING, allowNull: false }
}, {
  tableName: "state",
  timestamps: false
});

export default State;
