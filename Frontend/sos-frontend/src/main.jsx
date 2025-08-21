import React from "react";
import ReactDOM from "react-dom/client";
import { ChakraProvider, createSystem, defaultConfig } from "@chakra-ui/react";
import App from "./App.jsx";

// v3: se crea un "system" en vez de extendTheme
const system = createSystem(defaultConfig, {
  // aquí puedes personalizar (tokens, recipes, condiciones, etc.)
  // por ahora lo dejamos vacío para probar
});

ReactDOM.createRoot(document.getElementById("root")).render(
  <React.StrictMode>
    {/* v3: se pasa con la prop "value", no "theme" */}
    <ChakraProvider value={system}>
      <App />
    </ChakraProvider>
  </React.StrictMode>
);
