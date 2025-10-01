import { DataTypes } from "sequelize";
import { sequelize } from "../sequelize.js";

const Playthrough = sequelize.define("Playthrough", {
  id_playthrough: { type: DataTypes.INTEGER, primaryKey: true, autoIncrement: true },
  id_student:     { type: DataTypes.INTEGER, allowNull: false },
  start_date:     { type: DataTypes.DATE, allowNull: true },
  end_date:       { type: DataTypes.DATE, allowNull: true },
  version:        { type: DataTypes.STRING, allowNull: true },
  notes:          { type: DataTypes.TEXT, allowNull: true },
  id_status:      { type: DataTypes.INTEGER, allowNull: true }
}, {
  tableName: "playthrough",
  timestamps: false
});

export default Playthrough;
