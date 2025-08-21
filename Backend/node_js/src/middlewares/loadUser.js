import { User, Role, State } from "../models/indexUser.js";

export async function loadUser(req, res, next) {
  try {
    const user = await User.findByPk(req.jwt.id, {
      include: [
        { model: Role,  as: "roleData",  attributes: ["id_role", "name"] },
        { model: State, as: "stateData", attributes: ["id_state", "name"] },
      ],
    });

    if (!user) return res.status(401).json({ error: "Usuario inexistente" });
    if (user.stateData?.name !== "active") {
      return res.status(403).json({ error: "Cuenta no activa" });
    }

    req.user = user;
    return next();
  } catch (e) {
    console.error("loadUser error:", e);
    return res.status(500).json({ error: "Error cargando usuario" });
  }
}
