// src/components/psychologist/PsychologistKPIBlock.jsx
import { Box, Heading, SimpleGrid, Text } from "@chakra-ui/react";
import { useEffect, useMemo, useState } from "react";

// ⚠️ Paths: este componente vive en /components/psychologist/
// y los KPI comunes están en /components/kpi/
import KPIStatCard from "./kpi/KPIStatCard.jsx";
import KPILineChartLite from "./kpi/KPILineChartLite.jsx";
import KPIBarChartLite from "./kpi/KPIBarChartLite.jsx";

// Los servicios están en /services/
import {
  getReactionTime,
  getInactivity,
  getRepetition,
  getAlertsForStudent,
  getCaseEvolution,
} from "../services/kpiService";

// === helpers ===
const safeNumber = (val, fallback = 0) => {
  const n = Number(val);
  return Number.isFinite(n) ? n : fallback;
};

const sumAny = (rows, fields) =>
  fields.reduce(
    (acc, f) =>
      acc + rows.reduce((s, r) => s + safeNumber(r?.[f]), 0),
    0
  );

const pickFirstField = (row, candidates, fallback = null) => {
  for (const c of candidates) if (row && row[c] != null) return row[c];
  return fallback;
};

// === componente principal ===
export default function PsychologistKPIBlock({ studentId }) {
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  const [reaction, setReaction] = useState([]);   // vistaTiempoReaccion
  const [inactivity, setInact] = useState([]);    // vistaInactividad
  const [repetition, setRep] = useState([]);      // vistaPatronesRepeticion
  const [alerts, setAlerts] = useState(null);     // vistaAlertasPorEstudiante
  const [evolution, setEvo] = useState([]);       // vistaEvolucionCasos

  useEffect(() => {
    if (!studentId) return;
    setLoading(true);
    setError(null);
    (async () => {
      try {
        const [rt, ina, rep, al, evo] = await Promise.all([
          getReactionTime({ studentId }),
          getInactivity({ studentId }),
          getRepetition({ studentId }),
          getAlertsForStudent(studentId),
          getCaseEvolution(studentId),
        ]);
        setReaction(rt || []);
        setInact(ina || []);
        setRep(rep || []);
        setAlerts(al || null);
        setEvo(evo || []);
      } catch (err) {
        console.error("[PsychologistKPIBlock] error:", err);
        setError("No se pudieron cargar los KPI clínicos");
      } finally {
        setLoading(false);
      }
    })();
  }, [studentId]);

  // === KPIs calculados ===

  // Tiempo de reacción promedio → ms
  // vistaTiempoReaccion devuelve `tiempo_reaccion_prom` en segundos
  const reactionAvgMs = useMemo(() => {
    if (!reaction?.length) return null;
    const secs = safeNumber(reaction[0].tiempo_reaccion_prom, null);
    if (secs == null) return null;
    return Math.round(secs * 1000); // convertir a ms
  }, [reaction]);

  // Periodos de inactividad totales
  const inactivityTotal = useMemo(() => {
    return sumAny(inactivity, [
      "inactividad_total",  // real
      "inactividad_prom",   // real
      "inactivity_periods",
      "periodos_inactividad",
      "inactivity",
    ]);
  }, [inactivity]);

  // Repetición total
  const repetitionTotal = useMemo(() => {
    return sumAny(repetition, [
      "acciones_total",  // real
      "acciones_prom",   // real
      "repeticiones",
      "repeticion",
      "patrones",
      "count",
      "total",
    ]);
  }, [repetition]);

  // Barras de repetición por categoría
  const repetitionBars = useMemo(() => {
    const raw = Array.isArray(repetition) ? repetition : [];
    return raw
      .map((r) => {
        const label = pickFirstField(
          r,
          ["tipo", "label", "category", "nombre"],
          "Acciones"
        );
        const value = safeNumber(
          pickFirstField(r, [
            "acciones_total",
            "acciones_prom",
            "count",
            "value",
            "repeticiones",
            "total",
          ], 0)
        );
        return { label, value };
      })
      .filter((i) => i.value > 0);
  }, [repetition]);

  // Serie de evolución del caso
  const evolutionSeries = useMemo(() => {
    if (!evolution?.length) return [];
    return evolution.map((r) => {
      const x = pickFirstField(r, ["fecha", "date", "dia"], "");
      let y = pickFirstField(
        r,
        ["bdi_score", "score_total", "total_score", "puntaje", "score"],
        null
      );
      let yNum = safeNumber(y, NaN);
      if (!Number.isFinite(yNum)) {
        const lvl = pickFirstField(
          r,
          ["bdi_level", "id_level", "nivel_id", "nivel_severidad", "nivel", "level"],
          null
        );
        yNum = safeNumber(lvl, 0);
      }
      return { x, y: yNum };
    });
  }, [evolution]);

  // Normaliza serie para gráficas
  const evolutionSeriesForChart = useMemo(() => {
    if (evolutionSeries.length === 1) {
      const p = evolutionSeries[0];
      return [p, { ...p }];
    }
    return evolutionSeries;
  }, [evolutionSeries]);

  // Resumen de alertas
  const alertsSummary = useMemo(() => {
    if (!alerts) return "—";
    if (alerts.total_alertas != null) return String(alerts.total_alertas);
    const entries = Object.entries(alerts)
      .filter(([_, v]) => typeof v === "number")
      .map(([k, v]) => `${k}:${v}`)
      .join(" · ");
    return entries || "—";
  }, [alerts]);

  // === DEBUG LOGS ===
  useEffect(() => {
    console.group("🔎 PsychologistKPIBlock Debug");
    console.log("studentId:", studentId);

    console.log("➡️ Raw reaction:", reaction);
    console.log("➡️ Raw inactivity:", inactivity);
    console.log("➡️ Raw repetition:", repetition);
    console.log("➡️ Raw alerts:", alerts);
    console.log("➡️ Raw evolution:", evolution);

    console.log("📊 Calculated reactionAvgMs:", reactionAvgMs);
    console.log("📊 Calculated inactivityTotal:", inactivityTotal);
    console.log("📊 Calculated repetitionTotal:", repetitionTotal);

    console.log("📊 EvolutionSeries:", evolutionSeries);
    console.log("📊 EvolutionSeriesForChart:", evolutionSeriesForChart);
    console.log("📊 RepetitionBars:", repetitionBars);
    console.groupEnd();
  }, [
    studentId,
    reaction,
    inactivity,
    repetition,
    alerts,
    evolution,
    reactionAvgMs,
    inactivityTotal,
    repetitionTotal,
    evolutionSeries,
    evolutionSeriesForChart,
    repetitionBars,
  ]);

  return (
    <Box mt={8}>
      <Heading size="md" mb={4}>
        KPI Clínicos — Estudiante #{studentId}
      </Heading>

      {error && (
        <Box mb={4} p={3} borderWidth="1px" borderRadius="md" color="red.300">
          {error}
        </Box>
      )}

      <SimpleGrid columns={{ base: 1, md: 3 }} spacing={6} mb={6}>
        <KPIStatCard
          label="Tiempo de reacción prom."
          value={reactionAvgMs}
          unit="ms"
          loading={loading}
          format="int"
          help="Promedio de reacciones registradas"
        />
        <KPIStatCard
          label="Periodos de inactividad"
          value={inactivityTotal ?? 0}
          loading={loading}
          format="int"
        />
        <KPIStatCard
          label="Patrones de repetición"
          value={repetitionTotal ?? 0}
          loading={loading}
          format="int"
        />
      </SimpleGrid>

      <SimpleGrid columns={{ base: 1, md: 2 }} spacing={6}>
        <KPILineChartLite
          title="Evolución del caso"
          data={evolutionSeriesForChart}
          loading={loading}
        />
        <KPIBarChartLite
          title="Repetición por categoría"
          data={repetitionBars}
          loading={loading}
        />
      </SimpleGrid>

      <Box mt={6} p={4} borderWidth="1px" borderRadius="lg">
        <Text fontWeight="bold" mb={2}>
          Alertas del estudiante
        </Text>
        <Text color="gray.300">{alertsSummary}</Text>
      </Box>
    </Box>
  );
}
