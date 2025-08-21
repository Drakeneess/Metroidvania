import express from "express";
import jwt from "jsonwebtoken";
import { User, Role, State } from "../models/indexUser.js";

export const authRouter = express.Router();

authRouter.post("/login", async (req, res) => {
  const { email, password } = req.body;

  try {
    const user = await User.findOne({
      where: { email },
      include: [
        { model: Role,  as: "roleData",  attributes: ["id_role", "name"] },
        { model: State, as: "stateData", attributes: ["id_state", "name"] },
      ],
    });

    if (!user) return res.status(401).json({ error: "Usuario no encontrado" });

    // 🔒 SIN ENCRIPTACIÓN (solo mientras desarrollamos)
    if (password !== user.password) {
      return res.status(401).json({ error: "Credenciales inválidas" });
    }

    // Estado debe ser "active"
    if (user.stateData?.name !== "active") {
      return res.status(403).json({ error: "Cuenta no activa" });
    }

    // JWT (si no quieres token aún, comenta estas 4 líneas y ajusta tu front)
    if (!process.env.JWT_SECRET) {
      return res.status(500).json({ error: "Falta JWT_SECRET en .env" });
    }
    const token = jwt.sign(
      { id: user.id_user, role: user.roleData?.name, state: user.stateData?.name },
      process.env.JWT_SECRET,
      { expiresIn: "1h" }
    );

    return res.json({
      token, // si no quieres token ahora, puedes enviar null
      user: {
        id: user.id_user,
        full_name: user.full_name,
        email: user.email,
        role: user.roleData?.name || null,
        state: user.stateData?.name || null,
      },
    });
  } catch (err) {
    console.error("Login error:", err);
    return res.status(500).json({ error: "Error en servidor" });
  }
});
