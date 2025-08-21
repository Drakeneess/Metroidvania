import { authAdmin } from "../firebaseAdmin.js";

export async function verifyFirebaseToken(req, res, next) {
  const header = req.headers.authorization || "";
  const token = header.startsWith("Bearer ") ? header.slice(7) : null;

  if (!token) return res.status(401).json({ error: "Missing Bearer token" });

  try {
    const decoded = await authAdmin.verifyIdToken(token);
    req.firebaseUser = decoded; // { uid, email, ... }
    next();
  } catch (err) {
    console.error("verifyIdToken error:", err.message);
    res.status(401).json({ error: "Invalid or expired token" });
  }
}
