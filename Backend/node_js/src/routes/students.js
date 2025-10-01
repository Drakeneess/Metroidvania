import express from "express";
import {
  Student, BdiItem, BdiItemResponse,
  Playthrough, BdiResult, BdiTestResult, BdiLevel
} from "../models/indexStudent.js";
import { Op, fn, col, literal } from "sequelize";

export const studentsRouter = express.Router();

// Lista (puedes mantener la tuya)
studentsRouter.get("/", async (req, res, next) => {
  try {
    const students = await Student.findAll({ order: [["register_date", "DESC"]] });
    return res.json({ ok: true, students });
  } catch (err) {
    console.error("Error retrieving students:", err);
    return next(err);
  }
});

// Detalle FULL desde playthrough + bdi_result + bdi_test_result
studentsRouter.get("/:id_student/full", async (req, res, next) => {
  try {
    const { id_student } = req.params;
    const student = await Student.findByPk(id_student);
    if (!student) return res.status(404).json({ ok: false, error: "Estudiante no encontrado" });

    // 1) Todos los ítems
    const allItems = await BdiItem.findAll({
      attributes: ["id_item", "item_number", "title"],
      order: [["item_number", "ASC"]]
    });

    // 2) Buscar el último resultado del estudiante:
    //    por fecha de resultado (bdi_result.final_result_date) y, de respaldo, start_date del playthrough
    const latestResult = await BdiResult.findOne({
      include: [
        {
          model: Playthrough,
          where: { id_student },
          attributes: ["id_playthrough", "start_date", "end_date", "version", "id_status"]
        },
        { model: BdiLevel, as: "level", attributes: ["id_level", "level"] }
      ],
      order: [
        [literal("COALESCE(`BdiResult`.`final_result_date`, `Playthrough`.`start_date`)"), "DESC"]
      ]
    });

    // Si no hay resultados aún, responder estructura vacía válida para el front
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
          lastAnswerAt: null
        },
        items: { answered: [], unanswered: allItems.map(i => ({ id_item: i.id_item, item_number: i.item_number, title: i.title })) },
        outcomes: {
          all: await BdiLevel.findAll({ order: [["id_level", "ASC"]] }),
          matched: []
        },
        flat: allItems.map(i => ({ id_item: i.id_item, item_number: i.item_number, title: i.title, selected: null }))
      });
    }

    // 3) Respuestas del último resultado
    const tests = await BdiTestResult.findAll({
      where: { id_result: latestResult.id_bdi_result },
      include: [
        {
          model: BdiItemResponse,
          as: "answer",
          attributes: ["id_response", "id_item", "response", "response_symbol", "score"],
          include: [{ model: BdiItem, as: "item", attributes: ["id_item", "item_number", "title"] }]
        }
      ],
      order: [[{ model: BdiItemResponse, as: "answer" }, { model: BdiItem, as: "item" }, "item_number", "ASC"]]
    });

    // 4) Armar estructura para front
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
        response: a ? {
          id_response: a.id_response,
          response: a.response,
          response_symbol: a.response_symbol,
          score
        } : null
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
      (latestResult.items_responded != null && totalItems > 0)
        ? Math.round((latestResult.items_responded / totalItems) * 100)
        : (totalItems > 0 ? Math.round((answeredCount / totalItems) * 100) : 0);

    const levelsAll = await BdiLevel.findAll({ order: [["id_level", "ASC"]] });
    const matched = latestResult.level
      ? [latestResult.level]
      : [];

    return res.json({
      ok: true,
      student,
      stats: {
        totalItems,
        answeredCount,
        completion,
        totalScore: latestResult.score_total ?? totalScore,
        distribution,
        lastAnswerAt: lastAnswerAt ? new Date(lastAnswerAt).toISOString() : null
      },
      items: {
        answered: itemsAnswered,
        unanswered: itemsUnanswered
      },
      outcomes: {
        all: levelsAll,           // niveles disponibles (leve, moderado, grave, severo)
        matched                   // nivel asignado en bdi_result.id_level (si existe)
      },
      flat: allItems.map(i => ({
        id_item: i.id_item,
        item_number: i.item_number,
        title: i.title,
        selected: answeredMap.get(i.id_item)?.response || null
      })),
      playthrough: {
        id_playthrough: latestResult.Playthrough?.id_playthrough,
        start_date:     latestResult.Playthrough?.start_date,
        end_date:       latestResult.Playthrough?.end_date,
        version:        latestResult.Playthrough?.version,
        id_status:      latestResult.Playthrough?.id_status
      },
      result_meta: {
        id_bdi_result:     latestResult.id_bdi_result,
        final_result_date: latestResult.final_result_date
      }
    });
  } catch (err) {
    console.error("Error retrieving full student info:", err);
    return next(err);
  }
});
