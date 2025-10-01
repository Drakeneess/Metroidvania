// src/models/Student.js
import { DataTypes } from "sequelize";
import { sequelize } from "../sequelize.js";

const Student = sequelize.define("Student", {
  id_student: {
    type: DataTypes.INTEGER,
    primaryKey: true,
    autoIncrement: true
  },
  ci: {
    type: DataTypes.STRING,
    allowNull: false
  },
  full_name: {
    type: DataTypes.STRING,
    allowNull: false
  },
  age_range: {
    type: DataTypes.STRING,
    allowNull: false
  },
  register_date: {
    type: DataTypes.DATE,
    allowNull: false
  }
}, {
  tableName: "student",
  timestamps: false
});

export default Student;
