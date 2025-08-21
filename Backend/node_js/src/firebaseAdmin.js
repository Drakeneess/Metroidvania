import admin from "firebase-admin";

// Inicializar Firebase Admin con Service Account o con GOOGLE_APPLICATION_CREDENTIALS
if (!admin.apps.length) {
  admin.initializeApp({
    projectId: process.env.FIREBASE_PROJECT_ID, // ej: "shadowofsouls-8c119"
  });
}

export const authAdmin = admin.auth();
