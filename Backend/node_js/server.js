import app from "./src/app.js";

const PORT = Number(process.env.PORT) || 3000;

app.listen(PORT, "0.0.0.0", () => {
  console.log(`✅ API escuchando en puerto ${PORT}`);
});
