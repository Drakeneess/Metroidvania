import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'

// https://vitejs.dev/config/
export default defineConfig({
  plugins: [react()],
  server: {
    proxy: {
      // backend local actual
      "/auth": "http://localhost:4000",
      "/api": "http://localhost:4000",
      "/ingest": "http://localhost:4000",

      // ✅ Nuevo: backend Analytics en Railway para evitar CORS
      "/analytics": {
        target: "https://sosbehavioranalytics-production.up.railway.app",
        changeOrigin: true,
        secure: false,
        rewrite: (path) => path.replace(/^\/analytics/, "")
      }
    }
  }
})
