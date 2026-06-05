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
  if (Array.isArray(sessions) && sessions.length) {
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
