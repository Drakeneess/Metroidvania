export function requireRole(...allowedRoleNames) {
  return (req, res, next) => {
    const current = req.user?.roleData?.name; // nombre exacto en DB
    if (!current) return res.status(401).json({ error: "Sin rol" });
    if (!allowedRoleNames.includes(current)) {
      return res.status(403).json({ error: "Permisos insuficientes" });
    }
    next();
  };
}
