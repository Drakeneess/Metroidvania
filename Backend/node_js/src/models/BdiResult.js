import { DataTypes } from "sequelize";
import { sequelize } from "../sequelize.js";

const BdiResult = sequelize.define("BdiResult", {
  id_bdi_result:    { type: DataTypes.INTEGER, primaryKey: true, autoIncrement: true },
  id_playthrough:   { type: DataTypes.INTEGER, allowNull: false },
  score_total:      { type: DataTypes.INTEGER, allowNull: true },
  items_responded:  { type: DataTypes.INTEGER, allowNull: true },
  final_result_date:{ type: DataTypes.DATE, allowNull: true },
  id_level:         { type: DataTypes.INTEGER, allowNull: true }
}, {
  tableName: "bdi_result",
  timestamps: false
});

export default BdiResult;
