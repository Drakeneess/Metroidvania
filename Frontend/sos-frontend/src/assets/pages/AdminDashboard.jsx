import {
  Box,
  Heading,
  SimpleGrid,
  Text,
  Flex,
  Badge,
  VStack,
  HStack,
  Divider,
  Icon,
  Spinner,
  Alert,
  AlertIcon,
} from "@chakra-ui/react";
import { useEffect, useMemo, useState } from "react";
import { FiBarChart2, FiCalendar, FiActivity } from "react-icons/fi";

import KPIStatCard from "../components/kpi/KPIStatCard";
import KPILineChartLite from "../components/kpi/KPILineChartLite";
import KPIRangeToolbar from "../components/kpi/KPIRangeToolbar";

import {
  getActiveStudentsDaily,
  getAccessesDaily,
  getReportsEmittedDaily,
  getAvgSessionDuration,
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

function formatDateLabel(value) {
  if (!value) return null;

  try {
    return new Intl.DateTimeFormat("es-BO", {
      day: "2-digit",
      month: "short",
      year: "numeric",
    }).format(new Date(value));
  } catch {
    return value;
  }
}

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
      const [activeResponse, accessResponse, reportResponse, avgResponse] =
        await Promise.all([
          getActiveStudentsDaily({ from, to }),
          getAccessesDaily({ from, to }),
          getReportsEmittedDaily({ from, to }),
          getAvgSessionDuration(),
        ]);

      setActives(activeResponse || []);
      setAccesses(accessResponse || []);
      setReports(reportResponse || []);

      const seconds =
        avgResponse?.tiempo_promedio_sesion_seg != null
          ? Number(avgResponse.tiempo_promedio_sesion_seg)
          : null;

      setAvgSession(Number.isFinite(seconds) ? seconds : null);
    } catch (error) {
      console.error("Admin KPI error:", error);
      setError("No se pudieron cargar los KPI");
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    load();
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, []);

  const apply = () => load();

  const activesSeries = useMemo(
    () =>
      (actives || []).map((row) => ({
        x: row.dia,
        y: row.estudiantes_activos,
      })),
    [actives]
  );

  const accessesSeries = useMemo(
    () =>
      (accesses || []).map((row) => ({
        x: row.dia,
        y: row.accesos,
      })),
    [accesses]
  );

  const activeStudentsTotal = useMemo(
    () =>
      (actives || []).reduce(
        (total, row) => total + (row.estudiantes_activos || 0),
        0
      ),
    [actives]
  );

  const reportsTotal = useMemo(
    () =>
      (reports || []).reduce(
        (total, row) => total + (row.reportes_emitidos || 0),
        0
      ),
    [reports]
  );

  const accessesTotal = useMemo(
    () =>
      (accesses || []).reduce((total, row) => total + (row.accesos || 0), 0),
    [accesses]
  );

  const dateRangeLabel = useMemo(() => {
    const fromLabel = formatDateLabel(from);
    const toLabel = formatDateLabel(to);

    if (fromLabel && toLabel) return `${fromLabel} - ${toLabel}`;
    if (fromLabel) return `Desde ${fromLabel}`;
    if (toLabel) return `Hasta ${toLabel}`;

    return "Periodo general";
  }, [from, to]);

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
                Administración
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
              Panel administrativo
            </Heading>

            <Text color="gray.400" mt={2} maxW="760px">
              Monitoreo general de actividad, accesos, reportes emitidos y uso
              promedio del sistema.
            </Text>
          </Box>

          <DashboardPanel
            minW={{ base: "100%", md: "280px" }}
            p={4}
            bg="rgba(26, 26, 46, 0.78)"
          >
            <Text color="gray.500" fontSize="sm">
              Rango activo
            </Text>

            <Text color="white" fontWeight="bold">
              {dateRangeLabel}
            </Text>

            <Text color="brand.200" fontSize="sm" mt={1}>
              {loading ? "Actualizando métricas..." : "Métricas cargadas"}
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

        {/* Toolbar */}
        <DashboardPanel>
          <SectionHeader
            icon={FiCalendar}
            title="Rango de análisis"
            description="Filtra los indicadores administrativos por periodo."
          />

          <KPIRangeToolbar
            from={from}
            to={to}
            setFrom={setFrom}
            setTo={setTo}
            onApply={apply}
          />
        </DashboardPanel>

        {/* KPI principales */}
        <DashboardPanel>
          <SectionHeader
            icon={FiActivity}
            title="Resumen general"
            description="Indicadores principales del comportamiento del sistema."
            rightSlot={
              loading ? (
                <HStack color="gray.400">
                  <Spinner size="sm" />
                  <Text fontSize="sm">Cargando...</Text>
                </HStack>
              ) : null
            }
          />

          <SimpleGrid columns={{ base: 1, md: 2, xl: 4 }} spacing={5}>
            <KPIStatCard
              label="Estudiantes activos"
              value={activeStudentsTotal}
              loading={loading}
              error={error}
              format="int"
              help="Suma del periodo seleccionado"
            />

            <KPIStatCard
              label="Accesos al sistema"
              value={accessesTotal}
              loading={loading}
              error={error}
              format="int"
              help="Total de ingresos registrados"
            />

            <KPIStatCard
              label="Reportes emitidos"
              value={reportsTotal}
              loading={loading}
              error={error}
              format="int"
              help="Reportes generados en el periodo"
            />

            <KPIStatCard
              label="Tiempo prom. sesión"
              value={avgSession ? avgSession / 60 : null}
              unit="min"
              loading={loading}
              error={error}
              format="float"
              help={avgSession ? `${Math.round(avgSession)} seg` : undefined}
            />
          </SimpleGrid>
        </DashboardPanel>

        {/* Gráficos */}
        <DashboardPanel>
          <SectionHeader
            icon={FiBarChart2}
            title="Evolución diaria"
            description="Tendencia de estudiantes activos y accesos registrados por día."
          />

          <Divider borderColor="soul.softBorder" mb={5} />

          <SimpleGrid columns={{ base: 1, xl: 2 }} spacing={6}>
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
        </DashboardPanel>
      </VStack>
    </Box>
  );
}