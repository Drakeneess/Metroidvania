export function formatDateTime(date) {
  try {
    const d = new Date(date);
    return d.toLocaleString("es-BO", {
      timeZone: "America/La_Paz",
    });
  } catch {
    return "—";
  }
}

export function formatDateShort(date) {
  try {
    const d = new Date(date);
    return d.toLocaleDateString("es-BO", {
      timeZone: "America/La_Paz",
      month: "short",
      day: "numeric",
    });
  } catch {
    return "";
  }
}
