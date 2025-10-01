// src/services/studentService.js
import { api } from "../../lib/axiosInstance";

export async function fetchStudents() {
  const { data } = await api.get("/api/students");
  return data.students || [];
}

export async function fetchStudentFull(id_student) {
  const { data } = await api.get(`/api/students/${id_student}/full`);
  // data = { ok, student, stats, items, outcomes, flat }
  return data;
}
