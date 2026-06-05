// src/pages/TeacherDashboard.jsx

import {
  Box,
  Heading,
  SimpleGrid,
  Text,
  Flex,
  Badge,
  VStack,
  HStack,
  Select,
  Icon,
  Divider,
  Spinner,
  Alert,
  AlertIcon,
} from "@chakra-ui/react";
import { useEffect, useMemo, useState } from "react";
import { FiBookOpen, FiUser, FiBarChart2, FiActivity } from "react-icons/fi";

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

function DashboardPanel({ children, ...props }) {
  return (
    <Box
      bg="rgba(35, 41, 70, 0.84)"
      border="1px solid"
      borderColor="soul.border"
      borderRadius="2xl"
      boxShadow="soul"
      backdropFilter="blur(18px)"
      p={{ base: 4, md: 6 }}
      {...props}
    >
      {children}
    </Box>
  );
}

function SectionHeader({ icon, title, description, rightSlot }) {
  return (
    <Flex
      justify="space-between"
      align={{ base: "flex-start", md: "center" }}
      direction={{ base: "column", md: "row" }}
      gap={4}
      mb={5}
    >
      <HStack spacing={3} align="flex-start">
        <Flex
          w="42px"
          h="42px"
          align="center"
          justify="center"
          borderRadius="xl"
          bg="rgba(169, 112, 255, 0.16)"
          border="1px solid"
          borderColor="rgba(169, 112, 255, 0.28)"
          color="brand.200"
          flexShrink={0}
        >
          <Icon as={icon} boxSize={5} />
        </Flex>

        <Box>
          <Heading size="md" color="white">
            {title}
          </Heading>

          {description && (
            <Text color="gray.400" fontSize="sm" mt={1}>
              {description}
            </Text>
          )}
        </Box>
      </HStack>

      {rightSlot}
    </Flex>
  );
}

// helpers
const sumAny = (rows, fields) =>
  fields.reduce(
    (acc, field) =>
      acc +
      rows.reduce((sum, row) => sum + (Number(row?.[field]) || 0), 0),
    0
  );

const pickFirstField = (row, candidates, fallback = null) => {
  for (const candidate of candidates) {
    if (row && row[candidate] != null) return row[candidate];
  }

  return fallback;
};

export default function TeacherDashboard() {
  const [students, setStudents] = useState([]);
  const [studentId, setStudentId] = useState("");

  const [loading, setLoading] = useState(true);
  const [studentsLoading, setStudentsLoading] = useState(true);
  const [error, setError] = useState(null);

  const [weekly, setWeekly] = useState([]);
  const [freq, setFreq] = useState([]);
  const [exploration, setExploration] = useState([]);
  const [interactions, setInteractions] = useState([]);

  useEffect(() => {
    const loadStudents = async () => {
      setStudentsLoading(true);
      setError(null);

      try {
        const list = await fetchStudents();
        const safeList = Array.isArray(list) ? list : [];

        setStudents(safeList);

        if (safeList.length) {
          setStudentId(String(safeList[0].id_student));
        }
      } catch (error) {
        console.error("Error loading students:", error);
        setError("No se pudo cargar la lista de estudiantes");
      } finally {
        setStudentsLoading(false);
      }
    };

    loadStudents();
  }, []);

  useEffect(() => {
    if (!studentId) {
      setLoading(false);
      return;
    }

    const loadTeacherKPIs = async () => {
      setLoading(true);
      setError(null);

      try {
        const [weeklyData, frequencyData, explorationData, interactionsData] =
          await Promise.all([
            getSessionsWeekly({ studentId }),
            getSessionsFrequency({ studentId }),
            getExplorationByStudent({ studentId }),
            getInteractions({ studentId }),
          ]);

        setWeekly(Array.isArray(weeklyData) ? weeklyData : []);
        setFreq(Array.isArray(frequencyData) ? frequencyData : []);
        setExploration(Array.isArray(explorationData) ? explorationData : []);
        setInteractions(Array.isArray(interactionsData) ? interactionsData : []);
      } catch (error) {
        console.error("Teacher KPI error:", error);
        setError("No se pudieron cargar los KPIs del estudiante");
      } finally {
        setLoading(false);
      }
    };

    loadTeacherKPIs();
  }, [studentId]);

  const selectedStudent = useMemo(
    () =>
      students.find(
        (student) => String(student.id_student) === String(studentId)
      ),
    [students, studentId]
  );

  const totalSesiones = useMemo(() => {
    const fromFrequency = sumAny(freq, [
      "total_sesiones",
      "sesiones",
      "count",
      "num_sesiones",
    ]);

    if (fromFrequency) return fromFrequency;

    return sumAny(weekly, ["total_sesiones", "sesiones", "count"]);
  }, [freq, weekly]);

  const exploracionProm = useMemo(() => {
    if (!exploration?.length) return null;

    const values = exploration
      .map((row) =>
        Number(
          pickFirstField(
            row,
            [
              "exploration_score",
              "exploration_score_prom",
              "exploracion",
              "exploracion_promedio",
            ],
            null
          )
        )
      )
      .filter((value) => Number.isFinite(value));

    if (!values.length) return null;

    return values.reduce((a, b) => a + b, 0) / values.length;
  }, [exploration]);

  const interactionBars = useMemo(() => {
    const approaches = sumAny(interactions, [
      "aproximaciones",
      "approaches",
      "acercamientos",
      "social_interactions",
    ]);

    const avoidances = sumAny(interactions, [
      "evitaciones",
      "avoidances",
      "alejamientos",
    ]);

    return [
      { label: "Aprox.", value: approaches },
      { label: "Evita.", value: avoidances },
    ].filter((item) => item.value > 0 || interactions?.length > 0);
  }, [interactions]);

  const weeklySeries = useMemo(() => {
    if (!weekly?.length) return [];

    return weekly.map((row) => {
      const x = pickFirstField(
        row,
        ["anio_semana", "week", "semana", "label"],
        ""
      );

      const y =
        Number(
          pickFirstField(row, ["total_sesiones", "sesiones", "count"], 0)
        ) || 0;

      return { x, y };
    });
  }, [weekly]);

  const totalInteractions = useMemo(
    () => interactionBars.reduce((sum, item) => sum + item.value, 0),
    [interactionBars]
  );

  return (
    <Box maxW="1440px" mx="auto" px={{ base: 4, md: 8 }} py={{ base: 5, md: 8 }}>
      <VStack spacing={6} align="stretch">
        {/* Header */}
        <Flex
          justify="space-between"
          align={{ base: "flex-start", md: "center" }}
          direction={{ base: "column", md: "row" }}
          gap={4}
        >
          <Box>
            <HStack spacing={3} mb={3}>
              <Badge
                px={3}
                py={1}
                borderRadius="full"
                bg="rgba(169, 112, 255, 0.18)"
                color="brand.100"
                border="1px solid"
                borderColor="rgba(169, 112, 255, 0.32)"
              >
                Docente
              </Badge>

              <Badge
                px={3}
                py={1}
                borderRadius="full"
                bg="rgba(0, 187, 249, 0.14)"
                color="calm"
                border="1px solid"
                borderColor="rgba(0, 187, 249, 0.26)"
              >
                Shadow of Souls
              </Badge>
            </HStack>

            <Heading
              size={{ base: "lg", md: "xl" }}
              color="white"
              letterSpacing="-0.03em"
            >
              Panel docente
            </Heading>

            <Text color="gray.400" mt={2} maxW="760px">
              Seguimiento de sesiones, exploración e interacciones del estudiante
              dentro del videojuego.
            </Text>
          </Box>

          <DashboardPanel
            minW={{ base: "100%", md: "300px" }}
            p={4}
            bg="rgba(26, 26, 46, 0.78)"
          >
            <Text color="gray.500" fontSize="sm">
              Estudiante seleccionado
            </Text>

            <Text color="white" fontWeight="bold" noOfLines={1}>
              {selectedStudent?.full_name || "Sin estudiante seleccionado"}
            </Text>

            <Text color="brand.200" fontSize="sm" mt={1}>
              {selectedStudent?.id_student
                ? `ID #${selectedStudent.id_student}`
                : "Selecciona un registro"}
            </Text>
          </DashboardPanel>
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

        {/* Selector */}
        <DashboardPanel>
          <SectionHeader
            icon={FiUser}
            title="Selección de estudiante"
            description="Elige el estudiante para actualizar los indicadores docentes."
            rightSlot={
              studentsLoading ? (
                <HStack color="gray.400">
                  <Spinner size="sm" />
                  <Text fontSize="sm">Cargando estudiantes...</Text>
                </HStack>
              ) : null
            }
          />

          <Select
            value={studentId || ""}
            onChange={(event) => setStudentId(event.target.value)}
            maxW={{ base: "100%", md: "420px" }}
            isDisabled={studentsLoading || students.length === 0}
          >
            {students.length === 0 ? (
              <option value="">Sin estudiantes disponibles</option>
            ) : (
              students.map((student) => (
                <option key={student.id_student} value={student.id_student}>
                  #{student.id_student} — {student.full_name || student.ci || "Sin nombre"}
                </option>
              ))
            )}
          </Select>
        </DashboardPanel>

        {/* KPIs */}
        <DashboardPanel>
          <SectionHeader
            icon={FiActivity}
            title="Resumen del estudiante"
            description="Métricas principales asociadas al comportamiento de juego."
            rightSlot={
              loading ? (
                <HStack color="gray.400">
                  <Spinner size="sm" />
                  <Text fontSize="sm">Actualizando...</Text>
                </HStack>
              ) : null
            }
          />

          <SimpleGrid columns={{ base: 1, md: 3 }} spacing={5}>
            <KPIStatCard
              label="Sesiones registradas"
              value={totalSesiones ?? 0}
              loading={loading}
              error={error}
              format="int"
              help="Total calculado a partir de sesiones semanales o frecuencia"
            />

            <KPIStatCard
              label="Exploración promedio"
              value={exploracionProm}
              unit="%"
              loading={loading}
              error={error}
              format="float"
              help="Promedio en el periodo registrado"
            />

            <KPIStatCard
              label="Interacciones totales"
              value={totalInteractions}
              loading={loading}
              error={error}
              format="int"
              help="Aproximaciones y evitaciones registradas"
            />
          </SimpleGrid>
        </DashboardPanel>

        {/* Gráficas */}
        <DashboardPanel>
          <SectionHeader
            icon={FiBarChart2}
            title="Tendencias e interacciones"
            description="Visualización semanal y comparación de conductas de aproximación / evitación."
          />

          <Divider borderColor="soul.softBorder" mb={5} />

          <SimpleGrid columns={{ base: 1, xl: 2 }} spacing={6}>
            <KPILineChartLite
              title="Sesiones por semana"
              data={weeklySeries}
              loading={loading}
              error={error}
            />

            <KPIBarChartLite
              title="Interacciones"
              data={interactionBars}
              loading={loading}
              error={error}
            />
          </SimpleGrid>
        </DashboardPanel>
      </VStack>
    </Box>
  );
}