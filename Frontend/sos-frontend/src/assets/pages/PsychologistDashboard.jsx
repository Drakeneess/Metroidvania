// src/pages/PsychologistDashboard.jsx
import { Box, Heading, Text } from "@chakra-ui/react";
import { useEffect, useState } from "react";
import { fetchStudents, fetchStudentFull } from "../services/studentService";
import StudentTable from "../components/StudentTable";
import StudentFilters from "../components/StudentFilters";
import StudentDetailDrawer from "../components/student/StudentDetailDrawer.jsx";
import { useAuth } from "../auth/AuthContext";

// 👇 nuevo bloque KPI
import PsychologistKPIBlock from "../components/PsychologistKPIBlock.jsx";

export default function PsychologistDashboard() {
  const { user } = useAuth();
  const [students, setStudents] = useState([]);
  const [filtered, setFiltered] = useState([]);
  const [filters, setFilters] = useState({
    searchId: "",
    startDate: "",
    endDate: "",
    minAge: "",
    maxAge: ""
  });
  const [loading, setLoading] = useState(true);

  const [detailOpen, setDetailOpen] = useState(false);
  const [detailLoading, setDetailLoading] = useState(false);
  const [detailPayload, setDetailPayload] = useState(null);

  // 👇 selección para KPIs separados
  const [kpiStudentId, setKpiStudentId] = useState("");

  useEffect(() => {
    fetchStudents()
      .then((data) => {
        setStudents(data);
        setFiltered(data);
        if (data?.length) setKpiStudentId(String(data[0].id_student)); // default
      })
      .catch((err) => console.error("Error al cargar estudiantes:", err))
      .finally(() => setLoading(false));
  }, []);

  const applyFilters = () => {
    let filteredData = students;

    if (filters.searchId) {
      filteredData = filteredData.filter(s =>
        s.id_student.toString().includes(filters.searchId)
      );
    }

    if (filters.startDate) {
      filteredData = filteredData.filter(
        s => new Date(s.register_date) >= new Date(filters.startDate)
      );
    }

    if (filters.endDate) {
      filteredData = filteredData.filter(
        s => new Date(s.register_date) <= new Date(filters.endDate)
      );
    }

    if (filters.minAge) {
      filteredData = filteredData.filter(
        s => parseInt(s.age_range) >= parseInt(filters.minAge)
      );
    }

    if (filters.maxAge) {
      filteredData = filteredData.filter(
        s => parseInt(s.age_range) <= parseInt(filters.maxAge)
      );
    }

    setFiltered(filteredData);
    // si el actual ya no está en el filtro, resetea el selector KPI
    if (filteredData.length && !filteredData.find(s => String(s.id_student) === String(kpiStudentId))) {
      setKpiStudentId(String(filteredData[0].id_student));
    }
  };

  const handleSelectStudent = async (student) => {
    setDetailOpen(true);
    setDetailLoading(true);
    setDetailPayload(null);
    try {
      const payload = await fetchStudentFull(student.id_student);
      setDetailPayload(payload);
    } catch (e) {
      console.error("No se pudo cargar el detalle:", e);
    } finally {
      setDetailLoading(false);
    }
  };

  return (
    <Box p={8}>
      <Heading size="lg" mb={6}>Panel Psicólogo</Heading>

      {/* ====== Sección separada de KPI clínicos ====== */}
      <Box
        mt={10}
        mb={4}
        display="flex"
        gap="12px"
        flexWrap="wrap"
        alignItems="center"
      >
        <Text color="gray.300" fontWeight="semibold">KPIs del estudiante:</Text>
        <Box
          as="select"
          value={kpiStudentId || ""}
          onChange={(e) => setKpiStudentId(e.target.value)}
          sx={{
            padding: "8px 12px",
            background: "rgba(255,255,255,0.06)",
            borderRadius: "10px",
            border: "1px solid rgba(255,255,255,0.15)",
            color: "inherit",
          }}
        >
          {filtered.map(s => (
            <option key={s.id_student} value={s.id_student}>
              #{s.id_student} — {s.full_name || "Sin nombre"}
            </option>
          ))}
        </Box>
      </Box>

      {kpiStudentId ? (
        <PsychologistKPIBlock studentId={kpiStudentId} />
      ) : (
        <Box p={4} borderWidth="1px" borderRadius="lg" color="gray.300">
          Selecciona un estudiante para ver los KPI clínicos.
        </Box>
      )}
      <StudentFilters filters={filters} setFilters={setFilters} onApply={applyFilters} />
      <StudentTable students={filtered} loading={loading} onSelect={handleSelectStudent} />

      <StudentDetailDrawer
        isOpen={detailOpen}
        onClose={() => setDetailOpen(false)}
        loading={detailLoading}
        payload={detailPayload}
        authUser={user}
      />

    </Box>
  );
}
