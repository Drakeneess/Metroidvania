// app.js
import express from "express";
import cors from "cors";
import { verifyJWT } from "./middlewares/verifyJWT.js";
import { loadUser } from "./middlewares/loadUser.js";
import { adminSessionHeartbeat } from "./middlewares/adminSessionHeartbeat.js";
import { authRouter } from "./routes/auth.js";
import { protectedRouter } from "./routes/protected.js";
import { adminIngestRouter } from "./routes/adminIngest.js";
import { studentsRouter } from "./routes/students.js";
import { kpiRouterFactory } from "./routes/kpi.js";
import { sequelize } from "./sequelize.js";

const app = express();
app.use(cors());
app.use(express.json({ limit: "256kb" }));
app.set("trust proxy", true);

// Debug temporal
app.use((req, _res, next) => { console.log("REQ:", req.method, req.url); next(); });

// Rutas públicas
app.use("/auth", authRouter);

// Estudiantes
app.use("/api/students", verifyJWT, loadUser, studentsRouter);

// KPI
app.use("/api/kpi", kpiRouterFactory(sequelize));

// Rutas generales
app.use("/ingest/admin", adminIngestRouter);
app.use("/api", verifyJWT, loadUser, adminSessionHeartbeat, protectedRouter);

// Manejador global de errores
app.use((err, req, res, next) => {
  console.error("Unexpected error:", err);
  if (res.headersSent) return next(err);
  res.status(500).json({ ok: false, error: err.message || "Error interno" });
});

export default app;
