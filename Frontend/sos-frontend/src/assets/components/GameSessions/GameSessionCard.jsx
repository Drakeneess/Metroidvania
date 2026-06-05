// src/components/GameSessions/GameSessionCard.jsx

import { Badge, Box, Flex, HStack, Text } from "@chakra-ui/react";

function safeNumber(value, fallback = 0) {
  const number = Number(value);
  return Number.isFinite(number) ? number : fallback;
}

function getDurationMinutes(session) {
  const actions = safeNumber(session.actions);
  const apm = safeNumber(session.apm);

  if (session.duration_minutes != null) {
    return Math.max(1, Math.round(safeNumber(session.duration_minutes, 1)));
  }

  if (apm <= 0) return 1;

  return Math.max(1, Math.round(actions / apm));
}

function getClusterLabel(cluster) {
  if (cluster == null || cluster === "") return "—";
  return cluster;
}

export default function GameSessionCard({ session }) {
  const actions = safeNumber(session.actions);
  const apm = Math.round(safeNumber(session.apm));
  const durationMin = getDurationMinutes(session);
  const cluster = getClusterLabel(session.cluster);

  return (
    <Box
      borderRadius="18px"
      px={5}
      py={4}
      bg="rgba(255,255,255,0.76)"
      border="1px solid rgba(107,70,193,0.13)"
      boxShadow="0 2px 12px rgba(15,12,41,0.07)"
      backdropFilter="blur(8px) saturate(150%)"
      transition="transform 0.2s ease, box-shadow 0.2s ease, border-color 0.2s ease"
      _hover={{
        transform: "translateY(-2px)",
        boxShadow: "0 8px 22px rgba(15,12,41,0.12)",
        borderColor: "rgba(107,70,193,0.24)",
      }}
    >
      <Flex
        justify="space-between"
        align={{ base: "flex-start", md: "center" }}
        direction={{ base: "column", md: "row" }}
        gap={4}
      >
        <Box>
          <Text color="#1A202C" fontWeight="900" fontSize="md">
            Sesión #{session.id_session ?? session.id ?? "—"}
          </Text>

          <HStack mt={2} spacing={2} flexWrap="wrap">
            <Badge
              px={2}
              py={1}
              borderRadius="md"
              bg="rgba(107,70,193,0.10)"
              color="#6B46C1"
              border="1px solid rgba(107,70,193,0.18)"
            >
              {actions} acciones
            </Badge>

            <Badge
              px={2}
              py={1}
              borderRadius="md"
              bg="rgba(49,130,206,0.10)"
              color="#2B6CB0"
              border="1px solid rgba(49,130,206,0.18)"
            >
              {apm} APM
            </Badge>

            <Badge
              px={2}
              py={1}
              borderRadius="md"
              bg="rgba(56,161,105,0.10)"
              color="#2F855A"
              border="1px solid rgba(56,161,105,0.18)"
            >
              {durationMin} min
            </Badge>
          </HStack>
        </Box>

        <Box textAlign={{ base: "left", md: "right" }}>
          <Text color="#718096" fontSize="xs" fontWeight="800" textTransform="uppercase">
            Cluster
          </Text>

          <Text
            fontWeight="900"
            fontSize="xl"
            bgGradient="linear-gradient(135deg, #805AD5, #6B46C1)"
            bgClip="text"
            lineHeight="1.1"
          >
            {cluster}
          </Text>
        </Box>
      </Flex>
    </Box>
  );
}