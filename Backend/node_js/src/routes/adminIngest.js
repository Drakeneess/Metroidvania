import express from "express";
import { verifyJWT } from "../middlewares/verifyJWT.js";
import { loadUser } from "../middlewares/loadUser.js";
import { AdminSession } from "../models/indexUser.js";
import { Op } from "sequelize";

export const adminIngestRouter = express.Router();

// Config opcional: si quieres cerrar cualquier sesión previa abierta del mismo usuario al iniciar otra
const CLOSE_PREVIOUS_SESSIONS = true;

// Utilidad
const now = () => new Date();
const clientIp = (req) =>
  (req.headers["x-forwarded-for"]?.split(",")[0] ?? req.ip ?? "").toString().slice(0, 100);
const ua = (req) => String(req.headers["user-agent"] || "").slice(0, 512);

// POST /ingest/admin/session/start
adminIngestRouter.post("/session/start", verifyJWT, loadUser, async (req, res) => {
  try {
    const user = req.user; // viene de loadUser (User con roleData/stateData)
    const ts = now();

    if (CLOSE_PREVIOUS_SESSIONS) {
      await AdminSession.update(
        {
          ended_at: ts,
          last_seen_at: ts,
          duration_sec: AdminSession.sequelize.literal("TIMESTAMPDIFF(SECOND, started_at, NOW())"),
        },
        { where: { id_user: user.id_user, ended_at: { [Op.is]: null } } }
      );
    }

    const session = await AdminSession.create({
      id_user: user.id_user,
      started_at: ts,
      last_seen_at: ts,
      ended_at: null,
      duration_sec: null,
      ip_address: clientIp(req),
      user_agent: ua(req),
    });

    res.json({ ok: true, sessionId: session.session_id, startedAt: ts });
  } catch (err) {
    console.error("[INGEST] start error:", err);
    res.status(500).json({ ok: false, error: err.message });
  }
});

// POST /ingest/admin/session/heartbeat
adminIngestRouter.post("/session/heartbeat", verifyJWT, loadUser, async (req, res) => {
  try {
    const { sessionId } = req.body;
    if (!sessionId) return res.status(400).json({ ok: false, error: "Falta sessionId" });

    const session = await AdminSession.findByPk(sessionId);
    if (!session) return res.status(404).json({ ok: false, error: "Sesión no encontrada" });
    if (session.id_user !== req.user.id_user)
      return res.status(403).json({ ok: false, error: "Sesión no pertenece al usuario" });

    session.last_seen_at = now();
    await session.save();

    res.json({ ok: true, ts: session.last_seen_at });
  } catch (err) {
    console.error("[INGEST] heartbeat error:", err);
    res.status(500).json({ ok: false, error: err.message });
  }
});

// POST /ingest/admin/session/end
adminIngestRouter.post("/session/end", verifyJWT, loadUser, async (req, res) => {
  try {
    const { sessionId } = req.body;
    if (!sessionId) return res.status(400).json({ ok: false, error: "Falta sessionId" });

    const session = await AdminSession.findByPk(sessionId);
    if (!session) return res.status(404).json({ ok: false, error: "Sesión no encontrada" });
    if (session.id_user !== req.user.id_user)
      return res.status(403).json({ ok: false, error: "Sesión no pertenece al usuario" });

    const end = now();
    const duration = Math.max(0, Math.floor((end - session.started_at) / 1000));

    session.ended_at = end;
    session.last_seen_at = end;
    session.duration_sec = duration;
    await session.save();

    res.json({ ok: true, endedAt: end, durationSec: duration });
  } catch (err) {
    console.error("[INGEST] end error:", err);
    res.status(500).json({ ok: false, error: err.message });
  }
});
