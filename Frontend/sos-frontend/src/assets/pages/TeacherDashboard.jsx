// src/pages/TeacherDashboard.jsx
import { Box, Heading, SimpleGrid, Text } from "@chakra-ui/react";
import { useEffect, useMemo, useState } from "react";

// 👇 Usa rutas y extensión .jsx
import KPIStatCard from "../components/kpi/KPIStatCard.jsx";
import KPILineChartLite from "../components/kpi/KPILineChartLite.jsx";
import KPIBarChartLite from "../components/kpi/KPIBarChartLite.jsx";

import { fetchStudents } from "../services/studentService";
import {
  getSessionsFrequency,
  getSessionsWeekly,
  getExplorationByStudent,
  getInteractions,
} from "../services/kpiService";

// --- Debug: confirma que los imports son funciones
console.log("KPI imports types:", {
  KPIStatCard: typeof KPIStatCard,
  KPILineChartLite: typeof KPILineChartLite,
  KPIBarChartLite: typeof KPIBarChartLite,
});

// --- Wrapper seguro para evitar crash si algún import falla ---
const Safe = ({ Comp, props, label }) => {
  if (typeof Comp !== "function") {
    return (
      <Box borderWidth="1px" borderRadius="md" p={4} color="red.400">
        ⚠ Import inválido en <b>{label}</b>. Tipo recibido: <code>{String(typeof Comp)}</code>.  
        Revisa el <code>export default</code> y la ruta del archivo.
      </Box>
    );
  }
  return <Comp {...props} />;
};

// helpers
const sumAny = (rows, fields) =>
  fields.reduce(
    (acc, f) => acc + rows.reduce((s, r) => s + (Number(r?.[f]) || 0), 0),
    0
  );

const pickFirstField = (row, candidates, fallback = null) => {
  for (const c of candidates) if (row && row[c] != null) return row[c];
  return fallback;
};

export default function TeacherDashboard() {
  const [students, setStudents] = useState([]);
  const [studentId, setStudentId] = useState("");
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  // datasets
  const [weekly, setWeekly] = useState([]);
  const [freq, setFreq] = useState([]);
  const [exploration, setExploration] = useState([]);
  const [interactions, setInteractions] = useState([]);

  // cargar estudiantes
  useEffect(() => {
    (async () => {
      try {
        const list = await fetchStudents();
        setStudents(list);
        if (list?.length) setStudentId(String(list[0].id_student));
      } catch (e) {
        console.error("Error loading students:", e);
        setError("No se pudo cargar la lista de estudiantes");
      }
    })();
  }, []);

  // cargar KPIs cuando cambia studentId
  useEffect(() => {
    if (!studentId) return;
    (async () => {
      setLoading(true);
      setError(null);
      try {
        const [w, f, e, i] = await Promise.all([
          getSessionsWeekly({ studentId }),
          getSessionsFrequency({ studentId }),
          getExplorationByStudent({ studentId }),
          getInteractions({ studentId }),
        ]);

        console.group("🔎 TeacherDashboard Raw Data");
        console.log("studentId:", studentId);
        console.log("weekly:", w);
        console.log("frequency:", f);
        console.log("exploration:", e);
        console.log("interactions:", i);
        console.groupEnd();

        setWeekly(w || []);
        setFreq(f || []);
        setExploration(e || []);
        setInteractions(i || []);
      } catch (err) {
        console.error("Teacher KPI error:", err);
        setError("No se pudieron cargar los KPIs del estudiante");
      } finally {
        setLoading(false);
      }
    })();
  }, [studentId]);

  // KPIs calculados
  const totalSesiones = useMemo(() => {
    const a = sumAny(freq, ["total_sesiones", "sesiones", "count", "num_sesiones"]);
    return a || sumAny(weekly, ["total_sesiones", "sesiones", "count"]);
  }, [freq, weekly]);

  const exploracionProm = useMemo(() => {
    if (!exploration?.length) return null;
    const vals = exploration
      .map((r) =>
        Number(
          pickFirstField(
            r,
            ["exploration_score", "exploration_score_prom", "exploracion", "exploracion_promedio"],
            null
          )
        )
      )
      .filter((v) => Number.isFinite(v));
    if (!vals.length) return null;
    return vals.reduce((a, b) => a + b, 0) / vals.length;
  }, [exploration]);

  const interactionBars = useMemo(() => {
    const aprox = sumAny(interactions, ["aproximaciones", "approaches", "acercamientos", "social_interactions"]);
    const evita = sumAny(interactions, ["evitaciones", "avoidances", "alejamientos"]);
    const items = [
      { label: "Aprox.", value: aprox },
      { label: "Evita.", value: evita },
    ].filter((d) => d.value > 0 || interactions?.length > 0);
    return items;
  }, [interactions]);

  const weeklySeries = useMemo(() => {
    if (!weekly?.length) return [];
    return weekly.map((r) => {
      const x = pickFirstField(r, ["anio_semana", "week", "semana", "label"], "");
      const y = Number(pickFirstField(r, ["total_sesiones", "sesiones", "count"], 0)) || 0;
      return { x, y };
    });
  }, [weekly]);

  // Debug de KPIs ya procesados
  useEffect(() => {
    console.group("📊 TeacherDashboard KPIs");
    console.log("studentId:", studentId);
    console.log("totalSesiones:", totalSesiones);
    console.log("exploracionProm:", exploracionProm);
    console.log("interactionBars:", interactionBars);
    console.log("weeklySeries:", weeklySeries);
    console.groupEnd();
  }, [studentId, totalSesiones, exploracionProm, interactionBars, weeklySeries]);

  return (
    <Box p={8}>
      <Heading size="lg" mb={6}>Panel Docente</Heading>

      {/* Selector de estudiante */}
      <Box display="flex" gap="12px" flexWrap="wrap" mb={6} alignItems="center">
        <Text color="gray.300" fontWeight="semibold">Estudiante:</Text>
        <Box
          as="select"
          value={studentId || ""}
          onChange={(e) => setStudentId(e.target.value)}
          sx={{
            padding: "8px 12px",
            background: "rgba(255,255,255,0.06)",
            borderRadius: "10px",
            border: "1px solid rgba(255,255,255,0.15)",
            color: "inherit",
          }}
        >
          {students.map((s) => (
            <option key={s.id_student} value={s.id_student}>
              #{s.id_student} — {s.full_name || "Sin nombre"}
            </option>
          ))}
        </Box>
      </Box>

      {/* KPIs principales */}
      <SimpleGrid columns={{ base: 1, md: 3 }} spacing={6} mb={6}>
        <Safe
          label="KPIStatCard#1"
          Comp={KPIStatCard}
          props={{
            label: "Sesiones (último periodo)",
            value: totalSesiones ?? 0,
            loading,
            error,
            format: "int",
          }}
        />
        <Safe
          label="KPIStatCard#2"
          Comp={KPIStatCard}
          props={{
            label: "Exploración promedio",
            value: exploracionProm,
            unit: "%",
            loading,
            error,
            format: "float",
            help: "Promedio en el periodo seleccionado",
          }}
        />
        <Safe
          label="KPIStatCard#3"
          Comp={KPIStatCard}
          props={{
            label: "Interacciones totales",
            value: interactionBars.reduce((s, i) => s + i.value, 0),
            loading,
            error,
            format: "int",
          }}
        />
      </SimpleGrid>

      {/* Gráficas */}
      <SimpleGrid columns={{ base: 1, md: 2 }} spacing={6}>
        <Safe
          label="KPILineChartLite"
          Comp={KPILineChartLite}
          props={{ title: "Sesiones por semana", data: weeklySeries, loading, error }}
        />
        <Safe
          label="KPIBarChartLite#interactions"
          Comp={KPIBarChartLite}
          props={{ title: "Interacciones (Aproximación / Evitación)", data: interactionBars, loading, error }}
        />
      </SimpleGrid>
    </Box>
  );
}
