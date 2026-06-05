// src/components/Dashboard.jsx

import {
  Box,
  Heading,
  Text,
  SimpleGrid,
  VStack,
  HStack,
  Badge,
  Button,
  Flex,
  Icon,
} from "@chakra-ui/react";
import { Link as RouterLink } from "react-router-dom";
import { FiActivity, FiUsers, FiBookOpen, FiShield, FiArrowRight } from "react-icons/fi";
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

function QuickAccessCard({ icon, title, description, to, badge }) {
  return (
    <DashboardPanel
      transition="all 0.2s ease"
      _hover={{
        transform: "translateY(-2px)",
        borderColor: "brand.300",
        boxShadow: "glow",
      }}
    >
      <VStack align="stretch" spacing={4}>
        <Flex
          w="46px"
          h="46px"
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
          <HStack mb={2}>
            <Heading size="md" color="white">
              {title}
            </Heading>

            {badge && (
              <Badge
                bg="rgba(0, 187, 249, 0.14)"
                color="calm"
                border="1px solid"
                borderColor="rgba(0, 187, 249, 0.26)"
                borderRadius="full"
                px={2}
              >
                {badge}
              </Badge>
            )}
          </HStack>

          <Text color="gray.400" fontSize="sm">
            {description}
          </Text>
        </Box>

        <Button
          as={RouterLink}
          to={to}
          rightIcon={<FiArrowRight />}
          alignSelf="flex-start"
          size="sm"
        >
          Entrar
        </Button>
      </VStack>
    </DashboardPanel>
  );
}

export default function Dashboard() {
  const { user } = useAuth();

  const role = user?.role;

  const availableCards = [
    {
      icon: FiShield,
      title: "Panel administrativo",
      description:
        "Consulta métricas generales del sistema, actividad estudiantil, accesos y reportes emitidos.",
      to: "/dashboard/admin",
      badge: "Admin",
      roles: ["admin"],
    },
    {
      icon: FiActivity,
      title: "Panel psicólogo",
      description:
        "Revisa KPIs clínicos, consulta detalles individuales y analiza indicadores derivados del juego.",
      to: "/dashboard/psychologist",
      badge: "Psicología",
      roles: ["admin", "psychologist"],
    },
    {
      icon: FiBookOpen,
      title: "Panel docente",
      description:
        "Observa sesiones, exploración e interacciones del estudiante dentro de Shadow of Souls.",
      to: "/dashboard/teacher",
      badge: "Docente",
      roles: ["admin", "teacher"],
    },
  ].filter((card) => card.roles.includes(role));

  return (
    <Box maxW="1440px" mx="auto" px={{ base: 4, md: 8 }} py={{ base: 6, md: 10 }}>
      <VStack align="stretch" spacing={6}>
        {/* Hero */}
        <DashboardPanel
          position="relative"
          overflow="hidden"
          minH={{ base: "auto", md: "260px" }}
          display="flex"
          alignItems="center"
          _before={{
            content: '""',
            position: "absolute",
            inset: 0,
            bg: "radial-gradient(circle at top right, rgba(169,112,255,0.24), transparent 36%)",
            pointerEvents: "none",
          }}
        >
          <Box position="relative" maxW="820px">
            <HStack spacing={3} mb={4} flexWrap="wrap">
              <Badge
                px={3}
                py={1}
                borderRadius="full"
                bg="rgba(169, 112, 255, 0.18)"
                color="brand.100"
                border="1px solid"
                borderColor="rgba(169, 112, 255, 0.32)"
              >
                Shadow of Souls
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
                Dashboard
              </Badge>
            </HStack>

            <Heading
              size={{ base: "lg", md: "2xl" }}
              color="white"
              letterSpacing="-0.04em"
            >
              Bienvenido al sistema
            </Heading>

            <Text color="gray.300" mt={4} fontSize={{ base: "md", md: "lg" }} maxW="720px">
              Plataforma de seguimiento para visualizar actividad, sesiones,
              reportes e indicadores generados a partir de la experiencia de
              juego.
            </Text>

            <Text color="gray.500" mt={3} fontSize="sm">
              Usuario actual:{" "}
              <Text as="span" color="brand.200" fontWeight="bold">
                {user?.full_name || user?.username || "Usuario"}
              </Text>{" "}
              · Rol:{" "}
              <Text as="span" color="calm" fontWeight="bold">
                {role || "sin rol"}
              </Text>
            </Text>
          </Box>
        </DashboardPanel>

        {/* Accesos rápidos */}
        <Box>
          <HStack spacing={3} mb={4}>
            <Flex
              w="38px"
              h="38px"
              align="center"
              justify="center"
              borderRadius="xl"
              bg="rgba(169, 112, 255, 0.16)"
              border="1px solid"
              borderColor="rgba(169, 112, 255, 0.28)"
              color="brand.200"
            >
              <Icon as={FiUsers} boxSize={4} />
            </Flex>

            <Box>
              <Heading size="md" color="white">
                Accesos rápidos
              </Heading>
              <Text color="gray.500" fontSize="sm">
                Entra al panel correspondiente según tu rol.
              </Text>
            </Box>
          </HStack>

          {availableCards.length > 0 ? (
            <SimpleGrid columns={{ base: 1, md: 2, xl: 3 }} spacing={5}>
              {availableCards.map((card) => (
                <QuickAccessCard key={card.to} {...card} />
              ))}
            </SimpleGrid>
          ) : (
            <DashboardPanel>
              <Text color="gray.300" fontWeight="semibold">
                No hay paneles disponibles para este rol.
              </Text>

              <Text color="gray.500" fontSize="sm" mt={1}>
                Verifica los permisos asignados al usuario.
              </Text>
            </DashboardPanel>
          )}
        </Box>

        {/* Bloque informativo */}
        <SimpleGrid columns={{ base: 1, md: 3 }} spacing={5}>
          <DashboardPanel>
            <Text color="gray.400" fontSize="sm" fontWeight="semibold">
              Seguimiento
            </Text>
            <Heading size="md" color="white" mt={2}>
              Sesiones
            </Heading>
            <Text color="gray.500" fontSize="sm" mt={2}>
              Consulta registros de actividad por estudiante y evolución semanal.
            </Text>
          </DashboardPanel>

          <DashboardPanel>
            <Text color="gray.400" fontSize="sm" fontWeight="semibold">
              Indicadores
            </Text>
            <Heading size="md" color="white" mt={2}>
              KPIs
            </Heading>
            <Text color="gray.500" fontSize="sm" mt={2}>
              Visualiza métricas de exploración, accesos, reportes e interacciones.
            </Text>
          </DashboardPanel>

          <DashboardPanel>
            <Text color="gray.400" fontSize="sm" fontWeight="semibold">
              Análisis
            </Text>
            <Heading size="md" color="white" mt={2}>
              Reportes
            </Heading>
            <Text color="gray.500" fontSize="sm" mt={2}>
              Accede a información organizada para apoyo institucional y seguimiento.
            </Text>
          </DashboardPanel>
        </SimpleGrid>
      </VStack>
    </Box>
  );
}