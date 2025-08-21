import express from "express";
import { verifyJWT } from "../middlewares/verifyJWT.js";
import { loadUser } from "../middlewares/loadUser.js";
import { requireRole } from "../middlewares/requireRole.js";

export const protectedRouter = express.Router();

// Ruta: cualquier usuario activo logueado
protectedRouter.get("/me",
  verifyJWT,
  loadUser,
  (req, res) => {
    const u = req.user;
    res.json({
      id: u.id_user,
      full_name: u.full_name,
      email: u.email,
      role: u.roleData.name,
      state: u.stateData.name,
    });
  }
);

// Solo gabinete y admin
protectedRouter.get("/gabinete/data",
  verifyJWT,
  loadUser,
  requireRole("gabinete", "admin"),
  (req, res) => {
    res.json({ ok: true, data: "Solo gabinete/admin" });
  }
);

// Solo admin
protectedRouter.post("/admin/users",
  verifyJWT,
  loadUser,
  requireRole("admin"),
  (req, res) => {
    // Crear usuario, asignar roles, etc.
    res.json({ ok: true });
  }
);
