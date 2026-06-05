// src/services/studentService.js

import { api } from "../../lib/axiosInstance";

export async function fetchStudents() {
  const { data } = await api.get("/api/students");
  return data?.students || [];
}

export async function fetchStudentFull(idStudent: any) {
  if (!idStudent) return null;

  const { data } = await api.get(`/api/students/${idStudent}/full`);
  return data;
}