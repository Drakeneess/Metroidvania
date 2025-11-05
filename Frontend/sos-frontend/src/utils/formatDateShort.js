export function formatDateShort(dateStr) {
  if (!dateStr || typeof dateStr !== "string") return "—";

  // Normaliza formato (asegura "T" en medio)
  const normalized = dateStr.includes("T")
    ? dateStr
    : dateStr.replace(" ", "T");

  const d = new Date(normalized);

  if (isNaN(d.getTime())) {
    // algunos backends envían "YYYY/MM/DD" o timestamps
    try {
      const alt = new Date(dateStr);
      if (!isNaN(alt.getTime())) return alt.toLocaleString("es-BO");
      return "—";
    } catch {
      return "—";
    }
  }

  // Si todo va bien, formato corto local
  return d.toLocaleString("es-BO", {
    day: "2-digit",
    month: "short",
    year: "2-digit",
    hour: "2-digit",
    minute: "2-digit",
  });
}
