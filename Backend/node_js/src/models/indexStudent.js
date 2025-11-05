import Student from "./Student.js";
import BdiItem from "./BdiItem.js";
import BdiItemResponse from "./BdiItemResponse.js";
import Playthrough from "./Playthrough.js";
import BdiResult from "./BdiResult.js";
import BdiTestResult from "./BdiTestResult.js";
import BdiLevel from "./BdiLevel.js";
import AlertLevel from "./AlertLevel.js";
import AlertStudent from "./AlertStudent.js";

// BDI: ítem ↔ respuestas
BdiItem.hasMany(BdiItemResponse, { foreignKey: "id_item", sourceKey: "id_item", as: "responses" });
BdiItemResponse.belongsTo(BdiItem, { foreignKey: "id_item", targetKey: "id_item", as: "item" });

// Student ↔ Playthrough
Playthrough.belongsTo(Student, { foreignKey: "id_student", targetKey: "id_student" });
Student.hasMany(Playthrough, { foreignKey: "id_student", sourceKey: "id_student", as: "plays" });

// Playthrough ↔ BdiResult
BdiResult.belongsTo(Playthrough, { foreignKey: "id_playthrough", targetKey: "id_playthrough" });
Playthrough.hasMany(BdiResult, { foreignKey: "id_playthrough", sourceKey: "id_playthrough", as: "results" });

// BdiResult ↔ BdiLevel (severidad)
BdiResult.belongsTo(BdiLevel, { foreignKey: "id_level", targetKey: "id_level", as: "level" });

// BdiResult ↔ BdiTestResult (respuestas por ítem)
BdiTestResult.belongsTo(BdiResult, { foreignKey: "id_result", targetKey: "id_bdi_result" });
BdiResult.hasMany(BdiTestResult, { foreignKey: "id_result", sourceKey: "id_bdi_result", as: "tests" });

// BdiTestResult ↔ BdiItemResponse (y de ahí a BdiItem)
BdiTestResult.belongsTo(BdiItemResponse, { foreignKey: "id_response", targetKey: "id_response", as: "answer" });
// Nota: BdiItemResponse ya belongsTo(BdiItem) como "item" arriba.

// Student ↔ AlertStudent
Student.hasMany(AlertStudent, { foreignKey: "id_student", sourceKey: "id_student", as: "alerts" });
AlertStudent.belongsTo(Student, { foreignKey: "id_student", targetKey: "id_student" });

// AlertStudent ↔ AlertLevel
AlertStudent.belongsTo(AlertLevel, { foreignKey: "id_alert", targetKey: "id_alert", as: "level" });
AlertLevel.hasMany(AlertStudent, { foreignKey: "id_alert", sourceKey: "id_alert" });

export {
  Student, BdiItem, BdiItemResponse,
  Playthrough, BdiResult, BdiTestResult, BdiLevel,
  AlertLevel, AlertStudent
};
