// src/services/kpiService.js
import { api } from "../../lib/axiosInstance";

// Helper para query params
const qs = (o = {}) => {
  const entries = Object.entries(o).filter(([, v]) => v != null && v !== "");
  return entries.length ? "?" + new URLSearchParams(entries).toString() : "";
};

// ---- ADMIN / GENERALES ----
export async function getActiveStudentsDaily({ from, to } = {}) {
  const { data } = await api.get(`/api/kpi/active-students${qs({ from, to })}`);
  return data?.data || [];
}

export async function getAccessesDaily({ from, to } = {}) {
  const { data } = await api.get(`/api/kpi/accesses/daily${qs({ from, to })}`);
  return data?.data || [];
}

export async function getReportsEmittedDaily({ from, to } = {}) {
  const { data } = await api.get(`/api/kpi/reports/emitted/daily${qs({ from, to })}`);
  return data?.data || [];
}

export async function getReportsExportedDaily({ from, to } = {}) {
  const { data } = await api.get(`/api/kpi/reports/exported/daily${qs({ from, to })}`);
  return data?.data || [];
}

export async function getAvgSessionDuration() {
  const { data } = await api.get(`/api/kpi/sessions/avg-duration`);
  return data?.data || null;
}

// ---- TEACHER ----
export async function getSessionsFrequency({ studentId } = {}) {
  const { data } = await api.get(`/api/kpi/sessions/frequency${qs({ studentId })}`);
  return data?.data || [];
}

export async function getSessionsWeekly({ studentId } = {}) {
  const { data } = await api.get(`/api/kpi/sessions/weekly${qs({ studentId })}`);
  return data?.data || [];
}

export async function getExplorationByStudent({ studentId } = {}) {
  const { data } = await api.get(`/api/kpi/exploration/student${qs({ studentId })}`);
  return data?.data || [];
}

export async function getDecisions({ studentId } = {}) {
  const { data } = await api.get(`/api/kpi/decisions${qs({ studentId })}`);
  return data?.data || [];
}

export async function getInteractions({ studentId } = {}) {
  const { data } = await api.get(`/api/kpi/interactions${qs({ studentId })}`);
  return data?.data || [];
}

// ---- PSYCHOLOGIST ----
export async function getReactionTime({ studentId } = {}) {
  const { data } = await api.get(`/api/kpi/reaction-time${qs({ studentId })}`);
  return data?.data || [];
}

export async function getInactivity({ studentId } = {}) {
  const { data } = await api.get(`/api/kpi/inactivity${qs({ studentId })}`);
  return data?.data || [];
}

export async function getRepetition({ studentId } = {}) {
  const { data } = await api.get(`/api/kpi/repetition${qs({ studentId })}`);
  return data?.data || [];
}

export async function getAlertsDaily({ from, to } = {}) {
  const { data } = await api.get(`/api/kpi/alerts/daily${qs({ from, to })}`);
  return data?.data || [];
}

export async function getCaseEvolution(id_student) {
  const { data } = await api.get(`/api/kpi/cases/evolution/${id_student}`);
  return data?.data || [];
}

export async function getAlertsForStudent(id_student) {
  const { data } = await api.get(`/api/kpi/alerts/student/${id_student}`);
  return data?.data || null;
}
