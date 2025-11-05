// src/utils/export/exportToExcel.js
import * as XLSX from "xlsx";
import { buildGamerSummary } from "./exportToCSV";

export function exportToExcel({ payload, sessions = [] }) {
  const student = payload?.student || {};
  const stats = payload?.stats || {};
  const items = payload?.items || {};
  const answered = items.answered || [];

  const { total, apmAvg, clusterDominant } = buildGamerSummary(sessions);

  const studentSheet = XLSX.utils.aoa_to_sheet([
    ["Reporte del Estudiante – Shadow of Souls"],
    ["Nombre", student.full_name || ""],
    ["CI", student.ci || ""],
    ["Edad", student.age_range || ""],
    ["Registro", student.register_date ? new Date(student.register_date).toLocaleDateString() : ""],
    [],
    ["Resumen BDI-II"],
    ["Puntaje total", stats.totalScore ?? 0],
    ["Respondidas / Total", `${stats.answeredCount ?? 0} / ${stats.totalItems ?? 0}`],
    ["Completitud %", stats.completion ?? 0],
    ["Última respuesta", stats.lastAnswerAt ? new Date(stats.lastAnswerAt).toLocaleString() : ""],
    [],
    ["Resumen Gamer"],
    ["Total sesiones", total],
    ["APM promedio", Math.round(apmAvg)],
    ["Cluster dominante", clusterDominant],
  ]);

  const answersRows = [["#", "Ítem", "Respuesta", "Score"]];
  answered.forEach((r) => {
    const respText = r?.response?.response_symbol || r?.response?.response || "—";
    const score = typeof r?.response?.score === "number" ? r.response.score : "";
    answersRows.push([r.item_number, r.title || "—", respText, score]);
  });
  const answersSheet = XLSX.utils.aoa_to_sheet(answersRows);

  const sessionRows = [
    ["id_session", "APM", "Acciones", "Salud", "Cluster", "Tipos Únicos", "Interacciones", "Dashes", "Jumps", "Light Attacks", "Movement", "Stopped", "Inter Avg (s)"],
  ];
  (sessions || []).forEach((s) => {
    sessionRows.push([
      s.id_session,
      Math.round(s.apm || 0),
      s.actions ?? "",
      s.health ?? "",
      s.cluster ?? "",
      s.uniq_types ?? "",
      s.interactions ?? "",
      s.dashes ?? "",
      s.jumps ?? "",
      s.light_attacks ?? "",
      s.movement ?? "",
      s.stopped ?? "",
      s.inter_avg_s ?? "",
    ]);
  });
  const sessionsSheet = XLSX.utils.aoa_to_sheet(sessionRows);

  const wb = XLSX.utils.book_new();
  XLSX.utils.book_append_sheet(wb, studentSheet, "Estudiante");
  XLSX.utils.book_append_sheet(wb, answersSheet, "Respuestas BDI");
  XLSX.utils.book_append_sheet(wb, sessionsSheet, "Game Sessions");

  const filename = `reporte_estudiante_${student.id_student || "sos"}.xlsx`;
  XLSX.writeFile(wb, filename);
}
