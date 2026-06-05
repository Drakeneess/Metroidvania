export function formatDateShort(dateStr) {
  if (!dateStr || typeof dateStr !== "string") return "—";

  // 🔹 Normaliza formato (asegura "T" en medio)
  const normalized = dateStr.includes("T")
    ? dateStr
    : dateStr.replace(" ", "T");

  // 🔹 Intenta parsear normalmente
  let d = new Date(normalized);

  // 🔹 Fallback si formato raro o inválido
  if (isNaN(d.getTime())) {
    try {
      const alt = new Date(dateStr);
      if (!isNaN(alt.getTime())) d = alt;
      else return "—";
    } catch {
      return "—";
    }
  }

  // 🔹 Ajuste explícito a zona horaria Bolivia (UTC-4)
  return d.toLocaleString("es-BO", {
    timeZone: "America/La_Paz",
    day: "2-digit",
    month: "short",
    year: "2-digit",
    hour: "2-digit",
    minute: "2-digit",
  });
}
