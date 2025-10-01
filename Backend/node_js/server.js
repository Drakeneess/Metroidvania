import app from "./src/app.js";

const PORT = process.env.PORT || 4000;
app.listen(PORT, () => {
  console.log(`✅ API escuchando en http://localhost:${PORT}`);
});
