// Importa *default* porque tus modelos exportan default
import User from "./User.js";
import Role from "./Role.js";
import State from "./State.js";
import AdminSession from "./AdminSession.js";

/**
 * user_account.role (INT) --> role.id_role
 * user_account.state (INT) --> state.id_state
 */
User.belongsTo(Role,  { foreignKey: "id_role",  targetKey: "id_role",  as: "roleData"  });
User.belongsTo(State, { foreignKey: "id_state", targetKey: "id_state", as: "stateData" });

AdminSession.belongsTo(User, { foreignKey: "id_user", targetKey: "id_user", as: "userData" });

export { User, Role, State, AdminSession };
