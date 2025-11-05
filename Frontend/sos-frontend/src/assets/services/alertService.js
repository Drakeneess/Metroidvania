// src/services/alertService.js
import { api } from "../../lib/axiosInstance";

// 🔹 Obtener alerta actual de un estudiante
export async function getAlertForStudent(id_student) {
  try {
    const { data } = await api.get(`/api/alerts/student/${id_student}`);
    return data?.alert ?? null;
  } catch (err) {
    console.error("❌ Error en getAlertForStudent:", err.response?.status, err.response?.data);
    throw err; // importante para el catch del componente
  }
}

// 🔹 Obtener lista de niveles de alerta (metadatos del dropdown)
export async function getAlertLevels() {
  try {
    const { data } = await api.get(`/api/alerts/meta`);
    return data?.meta?.levels ?? [];
  } catch (err) {
    console.error("❌ Error en getAlertLevels:", err);
    return [];
  }
}

// 🔹 Crear nueva alerta para un estudiante
export async function createAlert({ id_student, id_alert }) {
  try {
    const { data } = await api.post(`/api/alerts`, { id_student, id_alert });
    return data?.alert ?? null;
  } catch (err) {
    console.error("❌ Error en createAlert:", err.response?.status, err.response?.data);
    throw err;
  }
}

// 🔹 Actualizar una alerta existente
export async function updateAlert({ id_alert_student, id_alert }) {
  try {
    const { data } = await api.put(`/api/alerts/${id_alert_student}`, { id_alert });
    return data?.alert ?? null;
  } catch (err) {
    console.error("❌ Error en updateAlert:", err.response?.status, err.response?.data);
    throw err;
  }
}

// 🔹 Eliminar alerta (si el psicólogo decide quitar la marca)
export async function deleteAlert(id_alert_student) {
  try {
    const { data } = await api.delete(`/api/alerts/${id_alert_student}`);
    return !!data?.ok;
  } catch (err) {
    console.error("❌ Error en deleteAlert:", err.response?.status, err.response?.data);
    throw err;
  }
}
