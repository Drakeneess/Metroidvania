import jwt from "jsonwebtoken";

export function verifyJWT(req, res, next) {
  const auth = req.headers.authorization || "";
  const token = auth.startsWith("Bearer ") ? auth.slice(7) : null;

  if (!token) return res.status(401).json({ error: "Falta token" });
  if (!process.env.JWT_SECRET) return res.status(500).json({ error: "Falta JWT_SECRET" });

  try {
    req.jwt = jwt.verify(token, process.env.JWT_SECRET); // { id, role, state } (según tu login)
    return next();
  } catch (e) {
    return res.status(401).json({ error: "Token inválido o vencido" });
  }
}
