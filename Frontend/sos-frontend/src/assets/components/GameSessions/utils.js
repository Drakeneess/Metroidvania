export function formatDateTime(date) {
  try {
    return new Date(date).toLocaleString();
  } catch {
    return "—";
  }
}
export function formatDateShort(date) {
  try {
    return new Date(date).toLocaleDateString(undefined, {
      month: "short",
      day: "numeric",
    });
  } catch {
    return "";
  }
}
