// src/components/psychologist/PsychologistKPIBlock.jsx

import {
  Box,
  Heading,
  SimpleGrid,
  Text,
  Alert,
  AlertIcon,
  Badge,
  HStack,
  VStack,
  Spinner,
  Flex,
} from "@chakra-ui/react";
import { useEffect, useMemo, useState } from "react";

import KPIStatCard from "./kpi/KPIStatCard.jsx";
import KPILineChartLite from "./kpi/KPILineChartLite.jsx";
import KPIBarChartLite from "./kpi/KPIBarChartLite.jsx";

import {
  getReactionTime,
  getInactivity,
  getRepetition,
  getAlertsForStudent,
  getCaseEvolution,
} from "../services/kpiService";

const safeNumber = (value, fallback = 0) => {
  const number = Number(value);
  return Number.isFinite(number) ? number : fallback;
};

const sumAny = (rows, fields) =>
  fields.reduce(
    (acc, field) =>
      acc + rows.reduce((sum, row) => sum + safeNumber(row?.[field]), 0),
    0
  );

const pickFirstField = (row, candidates, fallback = null) => {
  for (const candidate of candidates) {
    if (row && row[candidate] != null) return row[candidate];
  }

  return fallback;
};

function AlertSummaryCard({ alerts, loading }) {
  const totalAlerts = useMemo(() => {
    if (!alerts) return null;

    if (alerts.total_alertas != null) {
      return safeNumber(alerts.total_alertas);
    }

    const numericValues = Object.values(alerts).filter(
      (value) => typeof value === "number"
    );

    if (!numericValues.length) return null;

    return numericValues.reduce((sum, value) => sum + value, 0);
  }, [alerts]);

  const entries = useMemo(() => {
    if (!alerts) return [];

    return Object.entries(alerts).filter(
      ([key, value]) =>
        typeof value === "number" &&
        !["id_student", "total_alertas"].includes(key)
    );
  }, [alerts]);

  return (
    <Box
      p={5}
      bg="rgba(35, 41, 70, 0.84)"
      border="1px solid"
      borderColor="soul.border"
      borderRadius="2xl"
      boxShadow="soul"
      backdropFilter="blur(18px)"
    >
      <Flex justify="space-between" align="center" gap={4} mb={3}>
        <Box>
          <Heading size="sm" color="white">
            Alertas del estudiante
          </Heading>

          <Text color="gray.500" fontSize="sm" mt={1}>
            Resumen de alertas asociadas al registro seleccionado.
          </Text>
        </Box>

        {loading ? (
          <Spinner size="sm" color="brand.200" />
        ) : (
          <Badge
            px={3}
            py={1}
            borderRadius="full"
            bg={
              totalAlerts && totalAlerts > 0
                ? "rgba(239, 71, 111, 0.14)"
                : "rgba(6, 214, 160, 0.14)"
            }
            color={totalAlerts && totalAlerts > 0 ? "danger" : "success"}
            border="1px solid"
            borderColor={
              totalAlerts && totalAlerts > 0
                ? "rgba(239, 71, 111, 0.30)"
                : "rgba(6, 214, 160, 0.28)"
            }
          >
            {totalAlerts ?? 0} alertas
          </Badge>
        )}
      </Flex>

      {!loading && entries.length > 0 ? (
        <HStack spacing={2} flexWrap="wrap">
          {entries.map(([key, value]) => (
            <Badge
              key={key}
              px={2}
              py={1}
              borderRadius="md"
              bg="rgba(169, 112, 255, 0.14)"
              color="brand.100"
              border="1px solid"
              borderColor="rgba(169, 112, 255, 0.28)"
            >
              {key}: {value}
            </Badge>
          ))}
        </HStack>
      ) : !loading ? (
        <Text color="gray.400" fontSize="sm">
          No hay alertas adicionales registradas.
        </Text>
      ) : null}
    </Box>
  );
}

export default function PsychologistKPIBlock({ studentId }) {
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  const [reaction, setReaction] = useState([]);
  const [inactivity, setInactivity] = useState([]);
  const [repetition, setRepetition] = useState([]);
  const [alerts, setAlerts] = useState(null);
  const [evolution, setEvolution] = useState([]);

  useEffect(() => {
    if (!studentId) return;

    let cancelled = false;

    const loadKPIs = async () => {
      setLoading(true);
      setError(null);

      try {
        const [
          reactionData,
          inactivityData,
          repetitionData,
          alertData,
          evolutionData,
        ] = await Promise.all([
          getReactionTime({ studentId }),
          getInactivity({ studentId }),
          getRepetition({ studentId }),
          getAlertsForStudent(studentId),
          getCaseEvolution(studentId),
        ]);

        if (cancelled) return;

        setReaction(Array.isArray(reactionData) ? reactionData : []);
        setInactivity(Array.isArray(inactivityData) ? inactivityData : []);
        setRepetition(Array.isArray(repetitionData) ? repetitionData : []);
        setAlerts(alertData || null);
        setEvolution(Array.isArray(evolutionData) ? evolutionData : []);
      } catch (error) {
        if (cancelled) return;

        console.error("[PsychologistKPIBlock] error:", error);
        setError("No se pudieron cargar los KPI clínicos");
      } finally {
        if (!cancelled) setLoading(false);
      }
    };

    loadKPIs();

    return () => {
      cancelled = true;
    };
  }, [studentId]);

  const reactionAvgMs = useMemo(() => {
    if (!reaction.length) return null;

    const seconds = safeNumber(reaction[0]?.tiempo_reaccion_prom, null);

    if (seconds == null) return null;

    return Math.round(seconds * 1000);
  }, [reaction]);

  const inactivityTotal = useMemo(
    () =>
      sumAny(inactivity, [
        "inactividad_total",
        "inactividad_prom",
        "inactivity_periods",
        "periodos_inactividad",
        "inactivity",
      ]),
    [inactivity]
  );

  const repetitionTotal = useMemo(
    () =>
      sumAny(repetition, [
        "acciones_total",
        "acciones_prom",
        "repeticiones",
        "repeticion",
        "patrones",
        "count",
        "total",
      ]),
    [repetition]
  );

  const repetitionBars = useMemo(() => {
    return repetition
      .map((row) => {
        const label = pickFirstField(
          row,
          ["tipo", "label", "category", "nombre"],
          "Acciones"
        );

        const value = safeNumber(
          pickFirstField(
            row,
            [
              "acciones_total",
              "acciones_prom",
              "count",
              "value",
              "repeticiones",
              "total",
            ],
            0
          )
        );

        return { label, value };
      })
      .filter((item) => item.value > 0);
  }, [repetition]);

  const evolutionSeries = useMemo(() => {
    if (!evolution.length) return [];

    return evolution.map((row) => {
      const x = pickFirstField(row, ["fecha", "date", "dia"], "");

      const rawScore = pickFirstField(
        row,
        ["bdi_score", "score_total", "total_score", "puntaje", "score"],
        null
      );

      let y = safeNumber(rawScore, NaN);

      if (!Number.isFinite(y)) {
        const level = pickFirstField(
          row,
          ["bdi_level", "id_level", "nivel_id", "nivel_severidad", "nivel", "level"],
          0
        );

        y = safeNumber(level, 0);
      }

      return { x, y };
    });
  }, [evolution]);

  const evolutionSeriesForChart = useMemo(() => {
    if (evolutionSeries.length === 1) {
      const point = evolutionSeries[0];
      return [point, { ...point }];
    }

    return evolutionSeries;
  }, [evolutionSeries]);

  if (!studentId) {
    return (
      <Box
        p={5}
        border="1px dashed"
        borderColor="soul.border"
        borderRadius="xl"
        bg="rgba(255,255,255,0.035)"
      >
        <Text color="gray.400">
          Selecciona un estudiante para ver los KPI clínicos.
        </Text>
      </Box>
    );
  }

  return (
    <Box>
      <VStack align="stretch" spacing={5}>
        <Flex
          justify="space-between"
          align={{ base: "flex-start", md: "center" }}
          direction={{ base: "column", md: "row" }}
          gap={3}
        >
          <Box>
            <Heading size="md" color="white">
              KPI clínicos
            </Heading>

            <Text color="gray.500" fontSize="sm" mt={1}>
              Indicadores calculados para el estudiante #{studentId}.
            </Text>
          </Box>

          <Badge
            px={3}
            py={1}
            borderRadius="full"
            bg="rgba(169, 112, 255, 0.18)"
            color="brand.100"
            border="1px solid"
            borderColor="rgba(169, 112, 255, 0.32)"
          >
            Estudiante #{studentId}
          </Badge>
        </Flex>

        {error && (
          <Alert
            status="error"
            borderRadius="xl"
            bg="rgba(239, 71, 111, 0.12)"
            border="1px solid"
            borderColor="rgba(239, 71, 111, 0.28)"
            color="gray.100"
          >
            <AlertIcon />
            {error}
          </Alert>
        )}

        <SimpleGrid columns={{ base: 1, md: 3 }} spacing={5}>
          <KPIStatCard
            label="Tiempo de reacción prom."
            value={reactionAvgMs}
            unit="ms"
            loading={loading}
            error={error}
            format="int"
            help="Promedio de reacciones registradas"
          />

          <KPIStatCard
            label="Periodos de inactividad"
            value={inactivityTotal}
            loading={loading}
            error={error}
            format="int"
            help="Suma de periodos detectados"
          />

          <KPIStatCard
            label="Patrones de repetición"
            value={repetitionTotal}
            loading={loading}
            error={error}
            format="int"
            help="Acciones o patrones repetidos"
          />
        </SimpleGrid>

        <SimpleGrid columns={{ base: 1, xl: 2 }} spacing={5}>
          <KPILineChartLite
            title="Evolución del caso"
            data={evolutionSeriesForChart}
            loading={loading}
            error={error}
          />

          <KPIBarChartLite
            title="Repetición por categoría"
            data={repetitionBars}
            loading={loading}
            error={error}
          />
        </SimpleGrid>

        <AlertSummaryCard alerts={alerts} loading={loading} />
      </VStack>
    </Box>
  );
}