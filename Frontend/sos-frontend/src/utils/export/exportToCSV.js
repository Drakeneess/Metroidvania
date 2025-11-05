// src/utils/export/exportToCSV.js

function toCSVRow(fields = []) {
  return fields
    .map((f) => {
      const s = String(f ?? "").replace(/"/g, '""');
      return `"${s}"`;
    })
    .join(",");
}

function downloadFile(filename, content, mime = "text/csv;charset=utf-8;") {
  const blob = new Blob([content], { type: mime });
  const url = URL.createObjectURL(blob);
  const a = document.createElement("a");
  a.href = url;
  a.download = filename;
  a.click();
  URL.revokeObjectURL(url);
}

export function buildGamerSummary(sessions = []) {
  const total = sessions.length;
  const apmAvg = total ? sessions.reduce((a, s) => a + (Number(s.apm) || 0), 0) / total : 0;
  const clusterCount = new Map();
  sessions.forEach((s) => clusterCount.set(s.cluster, (clusterCount.get(s.cluster) || 0) + 1));
  let clusterDominant = "—";
  if (clusterCount.size) {
    clusterDominant = [...clusterCount.entries()].sort((a, b) => b[1] - a[1])[0][0];
  }
  return { total, apmAvg, clusterDominant };
}

export function exportToCSV({ payload, sessions = [] }) {
  const student = payload?.student || {};
  const stats = payload?.stats || {};
  const items = payload?.items || {};
  const answered = items.answered || [];

  const { total, apmAvg, clusterDominant } = buildGamerSummary(sessions);

  const lines = [];
  lines.push("Reporte del Estudiante – Shadow of Souls (CSV)");
  lines.push(toCSVRow(["Nombre", "CI", "Edad", "Registro"]));
  lines.push(
    toCSVRow([
      student.full_name,
      student.ci,
      student.age_range,
      student.register_date ? new Date(student.register_date).toLocaleDateString() : "",
    ])
  );
  lines.push("");

  lines.push("Resumen BDI-II");
  lines.push(toCSVRow(["Puntaje total", "Respondidas", "Total Ítems", "Completitud %", "Última respuesta"]));
  lines.push(
    toCSVRow([
      stats.totalScore ?? 0,
      stats.answeredCount ?? 0,
      stats.totalItems ?? 0,
      stats.completion ?? 0,
      stats.lastAnswerAt ? new Date(stats.lastAnswerAt).toLocaleString() : "",
    ])
  );
  lines.push("");

  lines.push("Resumen Gamer");
  lines.push(toCSVRow(["Total sesiones", "APM promedio", "Cluster dominante"]));
  lines.push(toCSVRow([total, Math.round(apmAvg), clusterDominant]));
  lines.push("");

  lines.push("Respuestas BDI-II");
  lines.push(toCSVRow(["#", "Ítem", "Respuesta", "Score"]));
  answered.forEach((r) => {
    const respText = r?.response?.response_symbol || r?.response?.response || "—";
    const score = typeof r?.response?.score === "number" ? r.response.score : "";
    lines.push(toCSVRow([r.item_number, r.title || "—", respText, score]));
  });
  lines.push("");

  if (Array.isArray(sessions) && sessions.length) {
    lines.push("Game Sessions");
    lines.push(
      toCSVRow([
        "id_session",
        "APM",
        "Acciones",
        "Salud",
        "Cluster",
        "Tipos Únicos",
        "Interacciones",
        "Dashes",
        "Jumps",
        "Light Attacks",
        "Movement",
        "Stopped",
        "Inter Avg (s)",
      ])
    );
    sessions.forEach((s) => {
      lines.push(
        toCSVRow([
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
        ])
      );
    });
  }

  const csv = lines.join("\n");
  downloadFile(`reporte_estudiante_${student.id_student || "sos"}.csv`, csv);
}
