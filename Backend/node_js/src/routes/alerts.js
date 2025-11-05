// src/routes/alerts.js
import express from "express";
import { AlertStudent, AlertLevel, Student } from "../models/indexStudent.js";
import { verifyJWT } from "../middlewares/verifyJWT.js";
import { loadUser } from "../middlewares/loadUser.js";
import { requireRole } from "../middlewares/requireRole.js";


export const alertsRouter = express.Router();

alertsRouter.use(verifyJWT, loadUser);
/* ============================================================
   🔹 1. Listar niveles de alerta (para cargar metadatos en el spinner)
   ============================================================ */
alertsRouter.get("/meta", async (_req, res, next) => {
  try {
    const levels = await AlertLevel.findAll({
      order: [["alert_priority", "DESC"]],
      attributes: ["id_alert", "alert_type", "alert_color", "alert_priority"],
    });

    res.json({
      ok: true,
      meta: {
        count: levels.length,
        levels: levels.map((l) => ({
          id: l.id_alert,
          type: l.alert_type,
          color: l.alert_color,
          priority: l.alert_priority,
        })),
      },
    });
  } catch (err) {
    next(err);
  }
});

/* ============================================================
   🔹 2. Obtener alerta actual de un estudiante
   ============================================================ */
alertsRouter.get("/student/:id_student", async (req, res, next) => {
  try {
    const { id_student } = req.params;

    const alert = await AlertStudent.findOne({
      where: { id_student },
      order: [["id_alert_student", "DESC"]],
      include: [
        {
          model: AlertLevel,
          as: "level",
          attributes: ["alert_type", "alert_color", "alert_priority"],
          required: false,
        },
      ],
    });

    // Caso: sin alerta => psicólogo NO revisó aún
    if (!alert) {
      return res.status(200).json({
        ok: true,
        alert: null,
      });
    }

    // Caso: alerta encontrada
    return res.status(200).json({
      ok: true,
      alert: {
        id_alert_student: alert.id_alert_student,
        id_alert: alert.id_alert,
        id_student: alert.id_student,
        type: alert.level?.alert_type || "Sin alerta",
        color: alert.level?.alert_color || "gray.300",
        priority: alert.level?.alert_priority ?? 0,
      },
    });
  } catch (err) {
    console.error("❌ Error en /student/:id_student =>", err);
    if (res.headersSent) return;     // <- clave
    return next(err);
  }
});

/* ============================================================
   🔹 3. Crear una alerta para un estudiante
   ============================================================ */
alertsRouter.post("/", async (req, res, next) => {
  try {
    const { id_student, id_alert } = req.body;
    if (!id_student || !id_alert)
      return res.status(400).json({ ok: false, error: "Faltan parámetros" });

    const student = await Student.findByPk(id_student);
    const level = await AlertLevel.findByPk(id_alert);
    if (!student) return res.status(404).json({ ok: false, error: "Estudiante no encontrado" });
    if (!level) return res.status(404).json({ ok: false, error: "Nivel de alerta no encontrado" });

    const created = await AlertStudent.create({ id_student, id_alert });

    res.json({
      ok: true,
      alert: {
        id_alert_student: created.id_alert_student,
        id_student,
        type: level.alert_type,
        color: level.alert_color,
        priority: level.alert_priority,
      },
    });
  } catch (err) {
    next(err);
  }
});

/* ============================================================
   🔹 4. Actualizar alerta existente
   ============================================================ */
alertsRouter.put("/:id_alert_student", async (req, res, next) => {
  try {
    const { id_alert_student } = req.params;
    const { id_alert } = req.body;

    const alert = await AlertStudent.findByPk(id_alert_student);
    if (!alert) return res.status(404).json({ ok: false, error: "Alerta no encontrada" });

    const level = await AlertLevel.findByPk(id_alert);
    if (!level) return res.status(404).json({ ok: false, error: "Nivel de alerta no encontrado" });

    await alert.update({ id_alert });

    res.json({
      ok: true,
      alert: {
        id_alert_student,
        id_student: alert.id_student,
        type: level.alert_type,
        color: level.alert_color,
        priority: level.alert_priority,
      },
    });
  } catch (err) {
    next(err);
  }
});

/* ============================================================
   🔹 5. Eliminar alerta
   ============================================================ */
alertsRouter.delete("/:id_alert_student", async (req, res, next) => {
  try {
    const { id_alert_student } = req.params;
    const found = await AlertStudent.findByPk(id_alert_student);
    if (!found) return res.status(404).json({ ok: false, error: "Alerta no encontrada" });

    await AlertStudent.destroy({ where: { id_alert_student } });
    res.json({ ok: true });
  } catch (err) {
    next(err);
  }
});
