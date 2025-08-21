import express from "express";
import cors from "cors";
import dotenv from "dotenv";
import { authRouter } from "./src/routes/auth.js";
import { protectedRouter } from "./src/routes/protected.js";

dotenv.config();
const app = express();
app.use(cors());
app.use(express.json());

app.use("/auth", authRouter);
app.use("/api", protectedRouter);

const PORT = process.env.PORT || 4000;
app.listen(PORT, () => console.log(`API escuchando en http://localhost:${PORT}`));
