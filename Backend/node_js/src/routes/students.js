import express from "express";
import {
  Student, BdiItem, BdiItemResponse,
  Playthrough, BdiResult, BdiTestResult, BdiLevel,
  AlertStudent, AlertLevel
} from "../models/indexStudent.js";
import { Op, fn, col, literal } from "sequelize";

export const studentsRouter = express.Router();


// ========================================================
// 🔹 GET /api/students
// Lista general de estudiantes (puedes extender con alertas o KPIs)
// ========================================================
studentsRouter.get("/", async (req, res, next) => {
  try {
    const students = await Student.findAll({
      include: [
        {
          model: AlertStudent,
          as: "alerts",
          include: [
            {
              model: AlertLevel,
              as: "level",
              attributes: ["alert_type", "alert_color", "alert_priority"],
            },
          ],
          required: false,
          separate: true, // para evitar duplicados
          order: [["id_alert_student", "DESC"]],
          limit: 1, // solo la alerta más reciente
        },
      ],
      order: [["register_date", "DESC"]],
    });

    const formatted = students.map((s) => {
      const alert = s.alerts?.[0];
      return {
        id_student: s.id_student,
        ci: s.ci,
        full_name: s.full_name,
        age_range: s.age_range,
        register_date: s.register_date,
        alert: alert
          ? {
              id_alert_student: alert.id_alert_student,
              id_student: s.id_student,
              id_alert: alert.id_alert,
              type: alert.level?.alert_type,
              color: alert.level?.alert_color,
              priority: alert.level?.alert_priority ?? 0,
            }
          : null,
      };
    });

    return res.json({ ok: true, students: formatted });
  } catch (err) {
    console.error("❌ Error en /api/students:", err);
    return next(err);
  }
});


// ========================================================
// 🔹 GET /api/students/:id_student/full
// Ficha completa del estudiante (juego, BDI, resultados, alerta)
// ========================================================
studentsRouter.get("/:id_student/full", async (req, res, next) => {
  try {
    const { id_student } = req.params;

    const student = await Student.findByPk(id_student);
    if (!student)
      return res.status(404).json({ ok: false, error: "Estudiante no encontrado" });

    // 1️⃣ Obtener todos los ítems del cuestionario
    const allItems = await BdiItem.findAll({
      attributes: ["id_item", "item_number", "title"],
      order: [["item_number", "ASC"]],
    });

    // 2️⃣ Buscar último resultado registrado (por fecha)
    const latestResult = await BdiResult.findOne({
      include: [
        {
          model: Playthrough,
          where: { id_student },
          attributes: ["id_playthrough", "start_date", "end_date", "version", "id_status"],
        },
        { model: BdiLevel, as: "level", attributes: ["id_level", "level"] },
      ],
      order: [
        [literal("COALESCE(`BdiResult`.`final_result_date`, `Playthrough`.`start_date`)"), "DESC"],
      ],
    });

    // Si no hay resultados aún, devolver estructura vacía
    if (!latestResult) {
      return res.json({
        ok: true,
        student,
        stats: {
          totalItems: allItems.length,
          answeredCount: 0,
          completion: 0,
          totalScore: 0,
          distribution: { "0": 0, "1": 0, "2": 0, "3": 0 },
          lastAnswerAt: null,
        },
        items: {
          answered: [],
          unanswered: allItems.map(i => ({
            id_item: i.id_item,
            item_number: i.item_number,
            title: i.title,
          })),
        },
        outcomes: {
          all: await BdiLevel.findAll({ order: [["id_level", "ASC"]] }),
          matched: [],
        },
        flat: allItems.map(i => ({
          id_item: i.id_item,
          item_number: i.item_number,
          title: i.title,
          selected: null,
        })),
      });
    }

    // 3️⃣ Cargar respuestas individuales del último resultado
    const tests = await BdiTestResult.findAll({
      where: { id_result: latestResult.id_bdi_result },
      include: [
        {
          model: BdiItemResponse,
          as: "answer",
          attributes: ["id_response", "id_item", "response", "response_symbol", "score"],
          include: [
            { model: BdiItem, as: "item", attributes: ["id_item", "item_number", "title"] },
          ],
        },
      ],
      order: [
        [{ model: BdiItemResponse, as: "answer" }, { model: BdiItem, as: "item" }, "item_number", "ASC"],
      ],
    });

    // 4️⃣ Procesar datos estadísticos
    const answeredMap = new Map();
    let totalScore = 0;
    const distribution = { "0": 0, "1": 0, "2": 0, "3": 0 };
    let lastAnswerAt = null;

    const itemsAnswered = tests.map(t => {
      const a = t.answer;
      const it = a?.item;
      const score = a?.score ?? 0;

      totalScore += score;
      const key = String(score);
      if (distribution[key] != null) distribution[key]++;

      if (t.result_date) {
        const ts = new Date(t.result_date).getTime();
        if (!lastAnswerAt || ts > lastAnswerAt) lastAnswerAt = ts;
      }

      const row = {
        id_item: it?.id_item,
        item_number: it?.item_number,
        title: it?.title,
        response: a
          ? {
              id_response: a.id_response,
              response: a.response,
              response_symbol: a.response_symbol,
              score,
            }
          : null,
      };
      if (row.id_item != null) answeredMap.set(row.id_item, row);
      return row;
    });

    const itemsUnanswered = allItems
      .filter(i => !answeredMap.has(i.id_item))
      .map(i => ({ id_item: i.id_item, item_number: i.item_number, title: i.title }));

    const answeredCount = itemsAnswered.length;
    const totalItems = allItems.length;
    const completion =
      latestResult.items_responded != null && totalItems > 0
        ? Math.round((latestResult.items_responded / totalItems) * 100)
        : totalItems > 0
        ? Math.round((answeredCount / totalItems) * 100)
        : 0;

    const levelsAll = await BdiLevel.findAll({ order: [["id_level", "ASC"]] });
    const matched = latestResult.level ? [latestResult.level] : [];

    // 5️⃣ Buscar alerta activa (si existe)
    const alert = await AlertStudent.findOne({
      where: { id_student },
      include: [
        {
          model: AlertLevel,
          as: "alertLevel", // 🔹 alias único para evitar conflicto con BdiLevel
          attributes: ["alert_type", "alert_color", "alert_priority"],
        },
      ],
      order: [["id_alert_student", "DESC"]],
    });

    // Construcción de respuesta
    const response = {
      ok: true,
      student,
      stats: {
        totalItems,
        answeredCount,
        completion,
        totalScore: latestResult.score_total ?? totalScore,
        distribution,
        lastAnswerAt: lastAnswerAt ? new Date(lastAnswerAt).toISOString() : null,
      },
      items: {
        answered: itemsAnswered,
        unanswered: itemsUnanswered,
      },
      outcomes: {
        all: levelsAll,
        matched,
      },
      flat: allItems.map(i => ({
        id_item: i.id_item,
        item_number: i.item_number,
        title: i.title,
        selected: answeredMap.get(i.id_item)?.response || null,
      })),
      playthrough: {
        id_playthrough: latestResult.Playthrough?.id_playthrough,
        start_date: latestResult.Playthrough?.start_date,
        end_date: latestResult.Playthrough?.end_date,
        version: latestResult.Playthrough?.version,
        id_status: latestResult.Playthrough?.id_status,
      },
      result_meta: {
        id_bdi_result: latestResult.id_bdi_result,
        final_result_date: latestResult.final_result_date,
      },
    };

    // Solo incluir alerta si existe (no se envía null)
    if (alert) {
      response.alert = {
        id_alert_student: alert.id_alert_student,
        type: alert.alertLevel.alert_type,
        color: alert.alertLevel.alert_color,
        priority: alert.alertLevel.alert_priority,
      };
    }

    return res.json(response);
  } catch (err) {
    console.error("Error retrieving full student info:", err);
    return next(err);
  }
});
