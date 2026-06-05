// src/components/GameSessions/GameSessionsSection.jsx

import { Box, Divider, SimpleGrid, Text, VStack } from "@chakra-ui/react";

import GameSessionCard from "./GameSessionCard";
import GameSessionsTable from "./GameSessionsTable";
import GameSessionChart from "./GameSessionChart";

function getSessionDate(session) {
  return session.start_time || session.startTime || session.created_at || null;
}

function sortSessionsByDateDesc(sessions) {
  return [...sessions].sort((a, b) => {
    const dateA = new Date(getSessionDate(a)).getTime();
    const dateB = new Date(getSessionDate(b)).getTime();

    return (Number.isFinite(dateB) ? dateB : 0) - (Number.isFinite(dateA) ? dateA : 0);
  });
}

function EmptyState({ children, tone = "muted" }) {
  const isPurple = tone === "purple";

  return (
    <Box
      color={isPurple ? "#6B46C1" : "#718096"}
      fontSize="sm"
      bg={isPurple ? "rgba(107,70,193,0.10)" : "rgba(255,255,255,0.72)"}
      border="1px solid"
      borderColor={isPurple ? "rgba(107,70,193,0.18)" : "rgba(107,70,193,0.12)"}
      px={4}
      py={4}
      borderRadius="16px"
      boxShadow="0 2px 10px rgba(15,12,41,0.06)"
      textAlign="center"
      fontWeight="700"
    >
      {children}
    </Box>
  );
}

export default function GameSessionsSection({ sessions = [], error }) {
  if (error === "NO_SESSIONS") {
    return (
      <EmptyState tone="purple">
        El estudiante no tiene sesiones registradas todavía.
      </EmptyState>
    );
  }

  if (error) {
    return (
      <EmptyState>
        No se pudieron cargar las sesiones: {error}
      </EmptyState>
    );
  }

  if (!sessions.length) {
    return <EmptyState>Sin sesiones registradas.</EmptyState>;
  }

  const sorted = sortSessionsByDateDesc(sessions);

  return (
    <Box
      display="grid"
      gap={6}
      bg="rgba(250,250,255,0.78)"
      border="1px solid rgba(107,70,193,0.12)"
      borderRadius="20px"
      p={{ base: 4, md: 5 }}
      boxShadow="0 4px 18px rgba(15,12,41,0.08)"
      backdropFilter="blur(8px) saturate(160%)"
      color="#1A202C"
    >
      <Box>
        <Text color="#1A202C" fontWeight="900" fontSize="md">
          Sesiones de juego
        </Text>

        <Text color="#718096" fontSize="sm" mt={1}>
          Últimas sesiones procesadas por el módulo de analytics.
        </Text>
      </Box>

      <SimpleGrid columns={{ base: 1, md: 3 }} spacing={3}>
        {sorted.slice(0, 3).map((session, index) => (
          <GameSessionCard
            key={session.id_session ?? session.id ?? index}
            session={session}
          />
        ))}
      </SimpleGrid>

      <Divider borderColor="rgba(107,70,193,0.14)" />

      <GameSessionChart sessions={sorted} />

      <Divider borderColor="rgba(107,70,193,0.14)" />

      <VStack align="stretch" spacing={3}>
        <Box>
          <Text color="#1A202C" fontWeight="900" fontSize="md">
            Historial de sesiones
          </Text>

          <Text color="#718096" fontSize="sm" mt={1}>
            Registro detallado de acciones, APM, salud y cluster.
          </Text>
        </Box>

        <GameSessionsTable sessions={sorted} />
      </VStack>
    </Box>
  );
}