// routes/kpi.js
import express from "express";
import { requireRole } from "../middlewares/requireRole.js";
import { verifyJWT } from "../middlewares/verifyJWT.js";
import { loadUser } from "../middlewares/loadUser.js";


/** Utils fechas */
const toISODate = (v) => {
  if (!v) return null;
  const d = new Date(v);
  if (Number.isNaN(+d)) return null;
  return d.toISOString().slice(0, 10);
};
const today = () => new Date().toISOString().slice(0, 10);
const daysAgo = (n) => {
  const d = new Date();
  d.setDate(d.getDate() - n);
  return d.toISOString().slice(0, 10);
};

// wrapper anti "headers already sent"
const asyncHandler = (fn) => (req, res, next) =>
  Promise.resolve(fn(req, res, next)).catch(next);

// 🔑 matrices de roles
const ADMIN_ROLES   = [2, 1]; // admin y psychologist
const TEACHER_ROLES = [3, 2, 1]; // teacher, admin, psychologist

export const kpiRouterFactory = (sequelize) => {
  const router = express.Router();

  // Base: autenticación + cargar usuario
  router.use(verifyJWT, loadUser);

  // helper de query
  const q = async (sql, replacements = {}) => {
    const [rows] = await sequelize.query(sql, { replacements });
    return rows;
  };

  // =========================
  //   ADMIN / GABINETE
  // =========================

  // 1) Estudiantes activos
  router.get("/active-students",
    requireRole(...ADMIN_ROLES),
    asyncHandler(async (req, res) => {
      const from = toISODate(req.query.from) || daysAgo(30);
      const to   = toISODate(req.query.to)   || today();
      const rows = await q(
        `SELECT * FROM vistaestudiantesactivosdiario
          WHERE dia BETWEEN :from AND :to
          ORDER BY dia`,
        { from, to }
      );
      return res.json({ ok: true, from, to, data: rows });
    })
  );

  // 11) Reportes emitidos
  router.get("/reports/emitted/daily",
    requireRole(...ADMIN_ROLES),
    asyncHandler(async (req, res) => {
      const from = toISODate(req.query.from) || daysAgo(30);
      const to   = toISODate(req.query.to)   || today();
      const rows = await q(
        `SELECT * FROM vistareportesemitidosdiario
          WHERE dia BETWEEN :from AND :to
          ORDER BY dia`,
        { from, to }
      );
      return res.json({ ok: true, from, to, data: rows });
    })
  );

  // 11b) Reportes exportados
  router.get("/reports/exported/daily",
    requireRole(...ADMIN_ROLES),
    asyncHandler(async (req, res) => {
      const from = toISODate(req.query.from) || daysAgo(30);
      const to   = toISODate(req.query.to)   || today();
      const rows = await q(
        `SELECT * FROM vistareportesexportadosdiario
          WHERE dia BETWEEN :from AND :to
          ORDER BY dia, format`,
        { from, to }
      );
      return res.json({ ok: true, from, to, data: rows });
    })
  );

  // 12) Evolución de casos
  router.get("/cases/evolution/:id_student",
    requireRole(...ADMIN_ROLES),
    asyncHandler(async (req, res) => {
      const id_student = Number(req.params.id_student);
      const rows = await q(
        `SELECT * FROM vistaevolucioncasos
          WHERE id_student = :sid
          ORDER BY fecha`,
        { sid: id_student }
      );
      return res.json({ ok: true, data: rows });
    })
  );

  // 13) Accesos
  router.get("/accesses/daily",
    requireRole(...ADMIN_ROLES),
    asyncHandler(async (req, res) => {
      const from = toISODate(req.query.from) || daysAgo(30);
      const to   = toISODate(req.query.to)   || today();
      const rows = await q(
        `SELECT * FROM vistaaccesosdiario
          WHERE dia BETWEEN :from AND :to
          ORDER BY dia`,
        { from, to }
      );
      return res.json({ ok: true, from, to, data: rows });
    })
  );

  router.get("/accesses/detail",
    requireRole(...ADMIN_ROLES),
    asyncHandler(async (req, res) => {
      const from = toISODate(req.query.from) || daysAgo(7);
      const to   = toISODate(req.query.to)   || today();
      const rows = await q(
        `SELECT * FROM vistaaccesosdetalle
          WHERE timestamp BETWEEN :from AND DATE_ADD(:to, INTERVAL 1 DAY)
          ORDER BY timestamp`,
        { from, to }
      );
      return res.json({ ok: true, from, to, data: rows });
    })
  );

  // =========================
  //   TEACHER (y superiores)
  // =========================

  // 2) Frecuencia de sesiones
  router.get("/sessions/frequency",
    requireRole(...TEACHER_ROLES),
    asyncHandler(async (req, res) => {
      const { studentId } = req.query;
      const base = `SELECT * FROM vistafrecuenciasesiones`;
      const rows = await q(
        studentId ? `${base} WHERE id_student = :sid` : base,
        studentId ? { sid: Number(studentId) } : {}
      );
      return res.json({ ok: true, data: rows });
    })
  );

  // 2b) Sesiones por semana
  router.get("/sessions/weekly",
    requireRole(...TEACHER_ROLES),
    asyncHandler(async (req, res) => {
      const { studentId } = req.query;
      const base = `SELECT * FROM vistasesionesporsemana ORDER BY anio_semana`;
      const rows = await q(
        studentId
          ? `SELECT * FROM vistasesionesporsemana WHERE id_student = :sid ORDER BY anio_semana`
          : base,
        studentId ? { sid: Number(studentId) } : {}
      );
      return res.json({ ok: true, data: rows });
    })
  );

  // 3) Tiempo promedio sesión (si lo querés mostrar a docentes también)
  router.get("/sessions/avg-duration",
    requireRole(...TEACHER_ROLES),
    asyncHandler(async (req, res) => {
      const rows = await q(`SELECT * FROM vistatiemposesionglobal`);
      return res.json({ ok: true, data: rows[0] || null });
    })
  );

  // 4) Exploración por estudiante
  router.get("/exploration/student",
    requireRole(...TEACHER_ROLES),
    asyncHandler(async (req, res) => {
      const { studentId } = req.query;
      const base = `SELECT * FROM vistaexploracionporestudiante`;
      const rows = await q(
        studentId ? `${base} WHERE id_student = :sid` : base,
        studentId ? { sid: Number(studentId) } : {}
      );
      return res.json({ ok: true, data: rows });
    })
  );

  // 5) Decisiones
  router.get("/decisions",
    requireRole(...TEACHER_ROLES),
    asyncHandler(async (req, res) => {
      const { studentId } = req.query;
      const base = `SELECT * FROM vistadecisiontendencia`;
      const rows = await q(
        studentId ? `${base} WHERE id_student = :sid` : base,
        studentId ? { sid: Number(studentId) } : {}
      );
      return res.json({ ok: true, data: rows });
    })
  );

  // 6) Interacciones sociales
  router.get("/interactions",
    requireRole(...TEACHER_ROLES),
    asyncHandler(async (req, res) => {
      const { studentId } = req.query;
      const base = `SELECT * FROM vistainteraccionessociales`;
      const rows = await q(
        studentId ? `${base} WHERE id_student = :sid` : base,
        studentId ? { sid: Number(studentId) } : {}
      );
      return res.json({ ok: true, data: rows });
    })
  );

  // 7) Tiempo de reacción
  router.get("/reaction-time",
  requireRole(...TEACHER_ROLES),
  asyncHandler(async (req, res) => {
    const { studentId } = req.query;
    const base = `SELECT * FROM vistatiemporeaccion`;
    const rows = await q(
      studentId ? `${base} WHERE id_student = :sid` : base,
      studentId ? { sid: Number(studentId) } : {}
    );

    console.log("[/reaction-time] studentId:", studentId, "rows:", rows); // 👈 log debug
    return res.json({ ok: true, data: rows });
  })
);


  // 8) Inactividad
  router.get("/inactivity",
    requireRole(...TEACHER_ROLES),
    asyncHandler(async (req, res) => {
      const { studentId } = req.query;
      const base = `SELECT * FROM vistainactividad`;
      const rows = await q(
        studentId ? `${base} WHERE id_student = :sid` : base,
        studentId ? { sid: Number(studentId) } : {}
      );
      return res.json({ ok: true, data: rows });
    })
  );

  // 9) Repetición
  router.get("/repetition",
    requireRole(...TEACHER_ROLES),
    asyncHandler(async (req, res) => {
      const { studentId } = req.query;
      const base = `SELECT * FROM vistapatronesrepeticion`;
      const rows = await q(
        studentId ? `${base} WHERE id_student = :sid` : base,
        studentId ? { sid: Number(studentId) } : {}
      );
      return res.json({ ok: true, data: rows });
    })
  );

  // 10) Alertas diarias (si NO deben verlo docentes, dejar sólo ADMIN_ROLES)
  router.get("/alerts/daily",
    requireRole(...ADMIN_ROLES),
    asyncHandler(async (req, res) => {
      const from = toISODate(req.query.from) || daysAgo(30);
      const to   = toISODate(req.query.to)   || today();
      const rows = await q(
        `SELECT * FROM vistaalertasgeneradasdiario
          WHERE dia BETWEEN :from AND :to
          ORDER BY dia, alert_level`,
        { from, to }
      );
      return res.json({ ok: true, from, to, data: rows });
    })
  );

  // Por estudiante: si quieres permitir a docentes ver SOLO sus asignados, necesitarías
  // lógica extra. Por ahora lo dejamos para admin/gabinete.
  router.get("/alerts/student/:id_student",
    requireRole(...ADMIN_ROLES),
    asyncHandler(async (req, res) => {
      const id_student = Number(req.params.id_student);
      const rows = await q(
        `SELECT * FROM vistaalertasporestudiante WHERE id_student = :sid`,
        { sid: id_student }
      );
      return res.json({ ok: true, data: rows[0] || null });
    })
  );

  return router;
};
