// /routes/authRouter.js
import express from "express";
import jwt from "jsonwebtoken";
import bcrypt from "bcrypt";

import { User, Role, State } from "../models/indexUser.js";

export const authRouter = express.Router();

authRouter.post("/login", async (req, res) => {
  const { email, password } = req.body;

  try {
    // Buscar usuario + rol + estado
    const user = await User.findOne({
      where: { email },
      include: [
        { model: Role,  as: "roleData",  attributes: ["id_role", "name"] },
        { model: State, as: "stateData", attributes: ["id_state", "name"] },
      ],
    });

    if (!user) return res.status(401).json({ error: "Usuario no encontrado" });

    // 1. Soporte contraseña plana (sistema antiguo)
    const isPlain = password === user.password;

    // 2. Soporte contraseña hasheada
    const isHash = await bcrypt.compare(password, user.password);

    if (!isPlain && !isHash) {
      return res.status(401).json({ error: "Credenciales inválidas" });
    }

    // 3. Migración automática a bcrypt
    if (isPlain) {
      const newHash = await bcrypt.hash(password, 12);
      user.password = newHash;
      await user.save();
    }

    // 4. Verificar estado
    if (user.stateData?.name !== "active") {
      return res.status(403).json({ error: "Cuenta inactiva" });
    }

    // 5. Generar JWT
    if (!process.env.JWT_SECRET) {
      return res.status(500).json({ error: "Falta JWT_SECRET en .env" });
    }

    const token = jwt.sign(
      {
        id: user.id_user,
        role: user.roleData?.name,
        state: user.stateData?.name,
      },
      process.env.JWT_SECRET,
      { expiresIn: "1h" }
    );

    // 6. Respuesta
    return res.json({
      ok: true,
      token,
      user: {
        id: user.id_user,
        full_name: user.full_name,
        email: user.email,
        role: user.roleData?.name,
        state: user.stateData?.name,
      },
    });

  } catch (err) {
    console.error("Login error:", err);
    return res.status(500).json({ error: "Error en servidor" });
  }
});
