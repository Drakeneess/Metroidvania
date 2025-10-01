import { DataTypes } from "sequelize";
import { sequelize } from "../sequelize.js";

const BdiTestResult = sequelize.define("BdiTestResult", {
  id_test:     { type: DataTypes.INTEGER, primaryKey: true, autoIncrement: true },
  id_result:   { type: DataTypes.INTEGER, allowNull: false }, // FK -> bdi_result.id_bdi_result
  id_response: { type: DataTypes.INTEGER, allowNull: false }, // FK -> bdi_item_response.id_response
  result_date: { type: DataTypes.DATE, allowNull: true }
}, {
  tableName: "bdi_test_result",
  timestamps: false
});

export default BdiTestResult;
