// src/components/student-detail/StudentStats.jsx

import {
  Box,
  SimpleGrid,
  Text,
  Progress,
  Flex,
  Badge,
} from "@chakra-ui/react";
import { safeNum } from "./utils";

function formatDateTime(value) {
  if (!value) return "—";

  const date = new Date(value);

  if (Number.isNaN(date.getTime())) return "—";

  return date.toLocaleString("es-BO", {
    day: "2-digit",
    month: "2-digit",
    year: "numeric",
    hour: "2-digit",
    minute: "2-digit",
  });
}

function getScoreState(score) {
  const value = safeNum(score);

  if (value >= 29) {
    return {
      label: "Alto",
      color: "#E53E3E",
      bg: "rgba(229, 62, 62, 0.12)",
      border: "rgba(229, 62, 62, 0.28)",
    };
  }

  if (value >= 20) {
    return {
      label: "Moderado",
      color: "#DD6B20",
      bg: "rgba(221, 107, 32, 0.12)",
      border: "rgba(221, 107, 32, 0.28)",
    };
  }

  if (value >= 14) {
    return {
      label: "Leve",
      color: "#D69E2E",
      bg: "rgba(214, 158, 46, 0.14)",
      border: "rgba(214, 158, 46, 0.30)",
    };
  }

  return {
    label: "Bajo",
    color: "#38A169",
    bg: "rgba(56, 161, 105, 0.12)",
    border: "rgba(56, 161, 105, 0.28)",
  };
}

function StatCard({ label, value, helper, children, accent = "#6B46C1" }) {
  return (
    <Box
      p={5}
      bg="rgba(255,255,255,0.76)"
      border="1px solid rgba(107,70,193,0.13)"
      borderRadius="18px"
      boxShadow="0 2px 12px rgba(15,12,41,0.07)"
      backdropFilter="blur(8px) saturate(150%)"
      minH="132px"
    >
      <Text color="#718096" fontSize="sm" fontWeight="800">
        {label}
      </Text>

      {children || (
        <Text
          mt={2}
          color={accent}
          fontSize="3xl"
          fontWeight="900"
          lineHeight="1"
          letterSpacing="-0.04em"
        >
          {value}
        </Text>
      )}

      {helper && (
        <Text color="#718096" fontSize="xs" mt={3} fontWeight="600">
          {helper}
        </Text>
      )}
    </Box>
  );
}

export default function StudentStats({ stats = {} }) {
  const totalScore = safeNum(stats.totalScore);
  const completion = safeNum(stats.completion);
  const answeredCount = safeNum(stats.answeredCount);
  const totalItems = safeNum(stats.totalItems);

  const scoreState = getScoreState(totalScore);

  return (
    <SimpleGrid columns={{ base: 1, md: 3 }} spacing={4}>
      <StatCard label="Puntaje total" accent={scoreState.color}>
        <Flex align="baseline" gap={3} mt={2}>
          <Text
            color={scoreState.color}
            fontSize="3xl"
            fontWeight="900"
            lineHeight="1"
            letterSpacing="-0.04em"
          >
            {totalScore}
          </Text>

          <Badge
            px={2}
            py={1}
            borderRadius="md"
            bg={scoreState.bg}
            color={scoreState.color}
            border="1px solid"
            borderColor={scoreState.border}
            fontWeight="800"
          >
            {scoreState.label}
          </Badge>
        </Flex>

        <Text color="#718096" fontSize="xs" mt={3} fontWeight="600">
          Resultado calculado del último registro BDI.
        </Text>
      </StatCard>

      <StatCard label="Avance">
        <Text
          mt={2}
          color="#6B46C1"
          fontSize="3xl"
          fontWeight="900"
          lineHeight="1"
          letterSpacing="-0.04em"
        >
          {completion}%
        </Text>

        <Progress
          value={completion}
          h="10px"
          mt={3}
          borderRadius="full"
          bg="rgba(0,0,0,0.07)"
          sx={{
            "& > div": {
              background: "linear-gradient(90deg, #6B46C1, #9F7AEA)",
            },
          }}
        />

        <Text color="#718096" fontSize="xs" mt={3} fontWeight="600">
          {answeredCount}/{totalItems} ítems respondidos
        </Text>
      </StatCard>

      <StatCard
        label="Última respuesta"
        value={formatDateTime(stats.lastAnswerAt)}
        helper="Fecha y hora del último ítem registrado."
        accent="#2B6CB0"
      />
    </SimpleGrid>
  );
}