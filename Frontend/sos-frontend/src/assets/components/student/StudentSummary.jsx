// src/components/student-detail/StudentSummary.jsx

import {
  Badge,
  Box,
  Flex,
  SimpleGrid,
  Text,
  Progress,
} from "@chakra-ui/react";

function safeNum(value, fallback = 0) {
  const number = Number(value);
  return Number.isFinite(number) ? number : fallback;
}

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
      gradient: "linear-gradient(135deg, #FC8181, #E53E3E)",
    };
  }

  if (value >= 20) {
    return {
      label: "Moderado",
      color: "#DD6B20",
      bg: "rgba(221, 107, 32, 0.12)",
      border: "rgba(221, 107, 32, 0.28)",
      gradient: "linear-gradient(135deg, #F6AD55, #DD6B20)",
    };
  }

  if (value >= 14) {
    return {
      label: "Leve",
      color: "#D69E2E",
      bg: "rgba(214, 158, 46, 0.14)",
      border: "rgba(214, 158, 46, 0.30)",
      gradient: "linear-gradient(135deg, #F6E05E, #D69E2E)",
    };
  }

  return {
    label: "Bajo",
    color: "#38A169",
    bg: "rgba(56, 161, 105, 0.12)",
    border: "rgba(56, 161, 105, 0.28)",
    gradient: "linear-gradient(135deg, #68D391, #38A169)",
  };
}

function SummaryCard({
  title,
  value,
  helper,
  gradient = "linear-gradient(135deg, #805AD5, #6B46C1)",
  children,
}) {
  return (
    <Box
      position="relative"
      overflow="hidden"
      borderRadius="18px"
      p={5}
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
      _before={{
        content: '""',
        position: "absolute",
        inset: 0,
        bg: "radial-gradient(circle at top right, rgba(107,70,193,0.10), transparent 42%)",
        pointerEvents: "none",
      }}
    >
      <Box position="relative">
        <Text
          fontSize="xs"
          color="#4A5568"
          fontWeight="800"
          textTransform="uppercase"
          letterSpacing="0.06em"
          mb={2}
        >
          {title}
        </Text>

        {children || (
          <Text
            fontWeight="900"
            fontSize="2xl"
            lineHeight="1.1"
            bgGradient={gradient}
            bgClip="text"
            letterSpacing="-0.03em"
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
    </Box>
  );
}

export default function StudentSummary({ stats = {} }) {
  const totalScore = safeNum(stats.totalScore);
  const answeredCount = safeNum(stats.answeredCount);
  const totalItems = safeNum(stats.totalItems);
  const completion = safeNum(stats.completion);

  const scoreState = getScoreState(totalScore);

  return (
    <SimpleGrid
      columns={{ base: 1, md: 3 }}
      spacing={4}
      mt={2}
    >
      <SummaryCard
        title="Puntaje total"
        gradient={scoreState.gradient}
        helper="Resultado del último registro BDI."
      >
        <Flex align="center" gap={3} flexWrap="wrap">
          <Text
            fontWeight="900"
            fontSize="3xl"
            lineHeight="1"
            bgGradient={scoreState.gradient}
            bgClip="text"
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
      </SummaryCard>

      <SummaryCard
        title="Avance"
        helper={`${answeredCount}/${totalItems} ítems respondidos`}
      >
        <Text
          fontWeight="900"
          fontSize="3xl"
          lineHeight="1"
          bgGradient="linear-gradient(135deg, #805AD5, #6B46C1)"
          bgClip="text"
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
              background: "linear-gradient(90deg, #805AD5, #6B46C1)",
            },
          }}
        />
      </SummaryCard>

      <SummaryCard
        title="Última respuesta"
        value={formatDateTime(stats.lastAnswerAt)}
        gradient="linear-gradient(135deg, #3182CE, #2B6CB0)"
        helper="Fecha y hora del último ítem registrado."
      />
    </SimpleGrid>
  );
}