// src/pages/PsychologistDashboard.jsx

import {
  Box,
  Heading,
  Text,
  Flex,
  Badge,
  Select,
  Spinner,
  VStack,
  HStack,
  Divider,
  Icon,
} from "@chakra-ui/react";
import { useEffect, useState } from "react";
import { FiActivity, FiFilter, FiUsers } from "react-icons/fi";

import { fetchStudents, fetchStudentFull } from "../services/studentService";
import StudentTable from "../components/StudentTable";
import StudentFilters from "../components/StudentFilters";
import StudentDetailDrawer from "../components/student/StudentDetailDrawer.jsx";
import PsychologistKPIBlock from "../components/PsychologistKPIBlock.jsx";
import { useAuth } from "../auth/AuthContext";

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
      gap={4}
      direction={{ base: "column", md: "row" }}
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

export default function PsychologistDashboard() {
  const { user } = useAuth();

  const [students, setStudents] = useState([]);
  const [filtered, setFiltered] = useState([]);

  const [filters, setFilters] = useState({
    searchId: "",
    startDate: "",
    endDate: "",
    minAge: "",
    maxAge: "",
  });

  const [loading, setLoading] = useState(true);

  const [detailOpen, setDetailOpen] = useState(false);
  const [detailLoading, setDetailLoading] = useState(false);
  const [detailPayload, setDetailPayload] = useState(null);

  const [kpiStudentId, setKpiStudentId] = useState("");

  useEffect(() => {
    fetchStudents()
      .then((data) => {
        const safeData = Array.isArray(data) ? data : [];

        setStudents(safeData);
        setFiltered(safeData);

        if (safeData.length) {
          setKpiStudentId(String(safeData[0].id_student));
        }
      })
      .catch((err) => console.error("Error al cargar estudiantes:", err))
      .finally(() => setLoading(false));
  }, []);

  const applyFilters = () => {
    let filteredData = [...students];

    if (filters.searchId) {
      filteredData = filteredData.filter((student) =>
        String(student.id_student).includes(filters.searchId)
      );
    }

    if (filters.startDate) {
      filteredData = filteredData.filter(
        (student) =>
          new Date(student.register_date) >= new Date(filters.startDate)
      );
    }

    if (filters.endDate) {
      filteredData = filteredData.filter(
        (student) =>
          new Date(student.register_date) <= new Date(filters.endDate)
      );
    }

    if (filters.minAge) {
      filteredData = filteredData.filter(
        (student) => parseInt(student.age_range) >= parseInt(filters.minAge)
      );
    }

    if (filters.maxAge) {
      filteredData = filteredData.filter(
        (student) => parseInt(student.age_range) <= parseInt(filters.maxAge)
      );
    }

    setFiltered(filteredData);

    const currentStudentStillVisible = filteredData.some(
      (student) => String(student.id_student) === String(kpiStudentId)
    );

    if (filteredData.length && !currentStudentStillVisible) {
      setKpiStudentId(String(filteredData[0].id_student));
    }

    if (!filteredData.length) {
      setKpiStudentId("");
    }
  };

  const handleSelectStudent = async (student) => {
    setDetailOpen(true);
    setDetailLoading(true);
    setDetailPayload(null);

    try {
      const payload = await fetchStudentFull(student.id_student);
      setDetailPayload(payload);
    } catch (error) {
      console.error("No se pudo cargar el detalle:", error);
    } finally {
      setDetailLoading(false);
    }
  };

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
                Psicología
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
              Panel del psicólogo
            </Heading>

            <Text color="gray.400" mt={2} maxW="720px">
              Consulta estudiantes, revisa indicadores clínicos derivados del
              videojuego y accede al detalle individual de cada registro.
            </Text>
          </Box>

          <DashboardPanel
            minW={{ base: "100%", md: "260px" }}
            p={4}
            bg="rgba(26, 26, 46, 0.78)"
          >
            <Text color="gray.500" fontSize="sm">
              Usuario actual
            </Text>

            <Text color="white" fontWeight="bold" noOfLines={1}>
              {user?.full_name || user?.username || "Psicólogo"}
            </Text>

            <Text color="brand.200" fontSize="sm" mt={1}>
              {user?.role || "psychologist"}
            </Text>
          </DashboardPanel>
        </Flex>

        {/* KPIs */}
        <DashboardPanel>
          <SectionHeader
            icon={FiActivity}
            title="KPIs clínicos del estudiante"
            description="Indicadores individuales calculados a partir de las sesiones registradas."
            rightSlot={
              <Select
                value={kpiStudentId || ""}
                onChange={(event) => setKpiStudentId(event.target.value)}
                maxW={{ base: "100%", md: "360px" }}
                isDisabled={loading || filtered.length === 0}
              >
                {filtered.map((student) => (
                  <option key={student.id_student} value={student.id_student}>
                    #{student.id_student} — {student.full_name || student.ci || "Sin nombre"}
                  </option>
                ))}
              </Select>
            }
          />

          <Divider borderColor="soul.softBorder" mb={5} />

          {loading ? (
            <Flex align="center" justify="center" minH="180px" color="gray.300">
              <Spinner mr={3} />
              <Text>Cargando estudiantes...</Text>
            </Flex>
          ) : kpiStudentId ? (
            <PsychologistKPIBlock studentId={kpiStudentId} />
          ) : (
            <Box
              p={5}
              border="1px dashed"
              borderColor="soul.border"
              borderRadius="xl"
              bg="rgba(255, 255, 255, 0.035)"
            >
              <Text color="gray.300" fontWeight="semibold">
                No hay estudiante seleccionado.
              </Text>

              <Text color="gray.500" fontSize="sm" mt={1}>
                Ajusta los filtros o selecciona un estudiante disponible para
                visualizar sus KPIs clínicos.
              </Text>
            </Box>
          )}
        </DashboardPanel>

        {/* Filtros */}
        <DashboardPanel>
          <SectionHeader
            icon={FiFilter}
            title="Filtros de búsqueda"
            description="Reduce la lista por ID, fecha de registro o rango de edad."
          />

          <StudentFilters
            filters={filters}
            setFilters={setFilters}
            onApply={applyFilters}
          />
        </DashboardPanel>

        {/* Tabla */}
        <DashboardPanel>
          <SectionHeader
            icon={FiUsers}
            title="Estudiantes registrados"
            description={`${filtered.length} de ${students.length} estudiantes visibles.`}
          />

          <StudentTable
            students={filtered}
            loading={loading}
            onSelect={handleSelectStudent}
          />
        </DashboardPanel>
      </VStack>

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