/// <reference types="vite/client" />

interface ImportMetaEnv {
  readonly VITE_API_URL: string
  // puedes añadir aquí otras VITE_*
}

interface ImportMeta {
  readonly env: ImportMetaEnv
}
