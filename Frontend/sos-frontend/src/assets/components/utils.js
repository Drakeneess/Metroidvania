export function formatDate(d) {
  try {
    return new Date(d).toLocaleDateString();
  } catch {
    return "—";
  }
}
export function safeNum(n, fallback = 0) {
  return typeof n === "number" && !Number.isNaN(n) ? n : fallback;
}
export function lighten(hex, amt = 0.5) {
  const clamp = (n) => Math.max(0, Math.min(255, n));
  const h = (hex || "#3182CE").replace("#", "");
  const r = clamp(Math.round(parseInt(h.substring(0, 2), 16) + 255 * amt));
  const g = clamp(Math.round(parseInt(h.substring(2, 4), 16) + 255 * amt));
  const b = clamp(Math.round(parseInt(h.substring(4, 6), 16) + 255 * amt));
  return `rgb(${r}, ${g}, ${b})`;
}
