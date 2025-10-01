// src/models/DecisionOutcome.js
import { DataTypes } from "sequelize";
import { sequelize } from "../sequelize.js";

const DecisionOutcome = sequelize.define("DecisionOutcome", {
  id_outcome: { type: DataTypes.INTEGER, primaryKey: true, autoIncrement: true },
  name:        { type: DataTypes.STRING,  allowNull: false },  // "Depresión Moderada"
  description: { type: DataTypes.TEXT,    allowNull: true  },
  min_score:   { type: DataTypes.INTEGER, allowNull: false },
  max_score:   { type: DataTypes.INTEGER, allowNull: false },
  narrative_flag:  { type: DataTypes.STRING, allowNull: true }, // "FLAG_DESPAIR"
  severity_level:  { type: DataTypes.STRING, allowNull: true }, // "mild|moderate|severe" (libre)
  active:      { type: DataTypes.BOOLEAN, allowNull: false, defaultValue: true }
}, {
  tableName: "decision_outcome",
  timestamps: false
});

export default DecisionOutcome;
