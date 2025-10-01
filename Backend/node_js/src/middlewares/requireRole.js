// middlewares/requireRole.js
export function requireRole(...allowedRoleIds) {
  return (req, res, next) => {
    const current = req.user?.roleData?.id_role;
    if (!current) {
      return res.status(401).json({ error: "Sin rol" });
    }
    if (!allowedRoleIds.includes(current)) {
      return res.status(403).json({ error: "Permisos insuficientes" });
    }
    next();
  };
}
