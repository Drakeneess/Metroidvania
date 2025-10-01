import { Box, Heading, SimpleGrid } from "@chakra-ui/react";
import { useEffect, useMemo, useState } from "react";
import KPIStatCard from "../components/kpi/KPIStatCard";
import KPILineChartLite from "../components/kpi/KPILineChartLite"; // <-- corregido
import KPIRangeToolbar from "../components/kpi/KPIRangeToolbar";
import {
  getActiveStudentsDaily,
  getAccessesDaily,
  getReportsEmittedDaily,
  getAvgSessionDuration
} from "../services/kpiService";

export default function AdminDashboard() {
  const [from, setFrom] = useState(null);
  const [to, setTo] = useState(null);

  const [loading, setLoading] = useState(true);
  const [actives, setActives] = useState([]);
  const [accesses, setAccesses] = useState([]);
  const [reports, setReports] = useState([]);
  const [avgSession, setAvgSession] = useState(null);
  const [error, setError] = useState(null);

  const load = async () => {
    setLoading(true);
    setError(null);
    try {
      const [A, X, R, avg] = await Promise.all([
        getActiveStudentsDaily({ from, to }),
        getAccessesDaily({ from, to }),
        getReportsEmittedDaily({ from, to }),
        getAvgSessionDuration(),
      ]);
      setActives(A || []);
      setAccesses(X || []);
      setReports(R || []);
      // viene como string tipo "2625.0000" -> número (segundos)
      const sec = avg?.tiempo_promedio_sesion_seg != null ? Number(avg.tiempo_promedio_sesion_seg) : null;
      setAvgSession(Number.isFinite(sec) ? sec : null);
    } catch (e) {
      console.error("Admin KPI error:", e);
      setError("No se pudieron cargar los KPI");
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => { load(); /* eslint-disable-next-line */ }, []);
  const apply = () => load();

  const activesSeries = useMemo(
    () => (actives || []).map(r => ({ x: r.dia, y: r.estudiantes_activos })),
    [actives]
  );
  const accessesSeries = useMemo(
    () => (accesses || []).map(r => ({ x: r.dia, y: r.accesos })),
    [accesses]
  );
  const reportsTotal = useMemo(
    () => (reports || []).reduce((s, r) => s + (r.reportes_emitidos || 0), 0),
    [reports]
  );

  return (
    <Box p={8}>
      <Heading size="lg" mb={4}>Panel Admin</Heading>

      <KPIRangeToolbar from={from} to={to} setFrom={setFrom} setTo={setTo} onApply={apply} />

      <SimpleGrid columns={[1, 3]} spacing={6} mb={6}>
        <KPIStatCard
          label="Estudiantes activos (suma periodo)"
          value={actives?.reduce((s,r)=>s+(r.estudiantes_activos||0),0)}
          loading={loading}
          error={error}
          format="int"
        />
        <KPIStatCard
          label="Reportes emitidos (periodo)"
          value={reportsTotal}
          loading={loading}
          error={error}
          format="int"
        />
        <KPIStatCard
          label="Tiempo prom. sesión"
          value={avgSession ? (avgSession / 60) : null} // minutos
          unit="min"
          loading={loading}
          error={error}
          format="float"
          help={avgSession ? `${Math.round(avgSession)} seg` : undefined}
        />
      </SimpleGrid>

      <SimpleGrid columns={[1, 2]} spacing={6}>
        <KPILineChartLite
          title="Activos por día"
          data={activesSeries}
          loading={loading}
          error={error}
        />
        <KPILineChartLite
          title="Accesos al sistema por día"
          data={accessesSeries}
          loading={loading}
          error={error}
        />
      </SimpleGrid>
    </Box>
  );
}
