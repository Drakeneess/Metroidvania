// src/firebaseAdmin.js
import admin from "firebase-admin";
import serviceAccount from "../secrets.json" with { type: "json" };

// Inicialización única del Admin SDK (Auth + Firestore)
if (!admin.apps.length) {
  admin.initializeApp({
    credential: admin.credential.cert(serviceAccount),
  });
  console.log("✅ Firebase Admin inicializado");
}

export const authAdmin = admin.auth();
export const db = admin.firestore();
