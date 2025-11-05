// src/utils/export/exportToPDF.js
import jsPDF from "jspdf";
import autoTable from "jspdf-autotable";  // ✅ Import correcto
import { buildGamerSummary } from "./exportToCSV";

export function exportToPDF({ payload, sessions = [], authUser }) {
  const student = payload?.student || {};
  const stats = payload?.stats || {};
  const items = payload?.items || {};
  const answered = items.answered || [];

  const { total, apmAvg, clusterDominant } = buildGamerSummary(sessions);

  const doc = new jsPDF({ unit: "mm", format: "a4" });

  const primary = "#1A202C";
  const subtle = "#4A5568";

  // Título
  doc.setFont("helvetica", "bold");
  doc.setFontSize(16);
  doc.setTextColor(primary);
  doc.text("Reporte del Estudiante – Shadow of Souls", 14, 18);
  doc.setFontSize(11);
  doc.setTextColor(subtle);
  doc.text(`Fecha: ${new Date().toLocaleString()}`, 14, 26);

  // Datos del estudiante
  autoTable(doc, {
    startY: 38,
    head: [["Campo", "Valor"]],
    body: [
      ["Nombre", student.full_name || ""],
      ["CI", student.ci || ""],
      ["Edad", student.age_range || ""],
      ["Registro", student.register_date ? new Date(student.register_date).toLocaleDateString() : ""],
    ],
    theme: "grid",
    styles: { fontSize: 10 },
    headStyles: { fillColor: [237, 242, 247], textColor: 0 },
  });

  let nextY = doc.lastAutoTable.finalY + 8;

  // Resumen BDI
  doc.setFontSize(12);
  doc.setFont("helvetica", "bold");
  doc.text("Resumen BDI-II", 14, nextY);
  autoTable(doc, {
    startY: nextY + 4,
    head: [["Métrica", "Valor"]],
    body: [
      ["Puntaje total", String(stats.totalScore ?? 0)],
      ["Respondidas / Total", `${stats.answeredCount ?? 0} / ${stats.totalItems ?? 0}`],
      ["Completitud %", String(stats.completion ?? 0)],
      ["Última respuesta", stats.lastAnswerAt ? new Date(stats.lastAnswerAt).toLocaleString() : "—"],
    ],
    theme: "grid",
    styles: { fontSize: 10 },
    headStyles: { fillColor: [237, 242, 247], textColor: 0 },
  });

  nextY = doc.lastAutoTable.finalY + 8;

  // Resumen Gamer
  doc.text("Resumen Gamer", 14, nextY);
  autoTable(doc, {
    startY: nextY + 4,
    head: [["Métrica", "Valor"]],
    body: [
      ["Total sesiones", String(total)],
      ["APM promedio", String(Math.round(apmAvg))],
      ["Cluster dominante", String(clusterDominant)],
    ],
    theme: "grid",
    styles: { fontSize: 10 },
    headStyles: { fillColor: [237, 242, 247], textColor: 0 },
  });

  nextY = doc.lastAutoTable.finalY + 8;

  // Respuestas BDI
  doc.text("Respuestas BDI-II", 14, nextY);
  autoTable(doc, {
    startY: nextY + 4,
    head: [["#", "Ítem", "Respuesta", "Score"]],
    body: answered.map((r) => [
      r.item_number,
      r.title || "—",
      r.response?.response_symbol || r.response?.response || "—",
      r.response?.score ?? "—",
    ]),
    theme: "grid",
    styles: { fontSize: 9 },
    headStyles: { fillColor: [237, 242, 247], textColor: 0 },
  });

  nextY = doc.lastAutoTable.finalY + 8;

  // Game Sessions
  doc.text("Game Sessions", 14, nextY);
  autoTable(doc, {
    startY: nextY + 4,
    head: [["ID", "APM", "Acciones", "Health", "Cluster", "Tipos", "Interacciones"]],
    body: sessions.map((s) => [
      s.id_session ?? "—",
      Math.round(s.apm || 0),
      s.actions ?? "—",
      s.health !== undefined ? `${(s.health * 100).toFixed(0)}%` : "—",
      s.cluster ?? "—",
      s.uniq_types ?? "—",
      s.interactions ?? "—",
    ]),
    theme: "grid",
    styles: { fontSize: 9 },
    headStyles: { fillColor: [237, 242, 247], textColor: 0 },
  });

  // Firma
  nextY = doc.lastAutoTable.finalY + 16;
  doc.text("______________________________", 14, nextY);
  doc.text("Psicólogo Responsable", 14, nextY + 6);
  doc.text(authUser?.full_name || "No disponible", 14, nextY + 12);
  authUser?.email && doc.text(authUser.email, 14, nextY + 18);

  // Guardar PDF
  doc.save(`reporte_estudiante_${student.id_student || "sos"}.pdf`);
}
