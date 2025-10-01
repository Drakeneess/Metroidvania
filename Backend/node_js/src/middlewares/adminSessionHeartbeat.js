import { AdminSession } from "../models/indexUser.js";

export async function adminSessionHeartbeat(req, res, next) {
  try {
    const sid = req.headers["x-session-id"];
    if (!sid || !req.user) return next();

    const session = await AdminSession.findByPk(sid);
    if (!session || session.id_user !== req.user.id_user) return next();

    const now = new Date();
    if (!session.last_seen_at || now - session.last_seen_at > 30 * 1000) {
      session.last_seen_at = now;
      await session.save();
    }
  } catch (e) {
    console.warn("[adminSessionHeartbeat] warning:", e.message);
  } finally {
    next();
  }
}
