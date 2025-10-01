import { DataTypes } from "sequelize";
import { sequelize } from "../sequelize.js";

const BdiLevel = sequelize.define("BdiLevel", {
  id_level: { type: DataTypes.INTEGER, primaryKey: true, autoIncrement: true },
  level:    { type: DataTypes.STRING, allowNull: false } // leve | moderado | grave | severo
}, {
  tableName: "bdi_level",
  timestamps: false
});

export default BdiLevel;
