import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'

// https://vitejs.dev/config/
export default defineConfig({
  plugins: [react()],
  server: {
    proxy: {
      // todo lo que empiece con /auth lo envía al backend 4000
      "/auth": "http://localhost:4000",
      "/api": "http://localhost:4000",
      "/ingest": "http://localhost:4000",
    },
  },
})
