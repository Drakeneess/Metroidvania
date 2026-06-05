// src/components/bdi/StudentOutcomeChips.jsx

import {
  Box,
  HStack,
  Tag,
  Text,
  Tooltip,
  VStack,
} from "@chakra-ui/react";

const severityToStyles = (severity) => {
  const normalized = String(severity || "").toLowerCase();

  if (normalized.includes("mild") || normalized.includes("leve")) {
    return {
      bg: "rgba(56, 161, 105, 0.12)",
      color: "#2F855A",
      border: "rgba(56, 161, 105, 0.28)",
    };
  }

  if (normalized.includes("moderate") || normalized.includes("moderado")) {
    return {
      bg: "rgba(214, 158, 46, 0.14)",
      color: "#B7791F",
      border: "rgba(214, 158, 46, 0.32)",
    };
  }

  if (normalized.includes("severe") || normalized.includes("grave")) {
    return {
      bg: "rgba(229, 62, 62, 0.12)",
      color: "#C53030",
      border: "rgba(229, 62, 62, 0.30)",
    };
  }

  return {
    bg: "rgba(49, 130, 206, 0.12)",
    color: "#2B6CB0",
    border: "rgba(49, 130, 206, 0.28)",
  };
};

function OutcomeTag({ outcome, subtle = false, fallback }) {
  const styles = severityToStyles(outcome?.severity_level);

  const label =
    outcome?.name ||
    outcome?.level ||
    fallback ||
    "Outcome";

  const text =
    outcome?.narrative_flag && !subtle
      ? `${label} · ${outcome.narrative_flag}`
      : label;

  const title = [
    outcome?.description,
    outcome?.min_score != null && outcome?.max_score != null
      ? `Rango: ${outcome.min_score}-${outcome.max_score}`
      : null,
  ]
    .filter(Boolean)
    .join(" · ");

  return (
    <Tooltip label={title} hasArrow isDisabled={!title}>
      <Tag
        px={3}
        py={1}
        borderRadius="full"
        bg={subtle ? "rgba(255,255,255,0.72)" : styles.bg}
        color={styles.color}
        border="1px solid"
        borderColor={styles.border}
        fontWeight="700"
        fontSize="xs"
        cursor={title ? "help" : "default"}
        boxShadow={subtle ? "0 1px 4px rgba(15,12,41,0.05)" : "none"}
      >
        {text}
      </Tag>
    </Tooltip>
  );
}

export default function StudentOutcomeChips({ matched = [], all = [] }) {
  if (!all.length && !matched.length) {
    return (
      <Box
        bg="rgba(255,255,255,0.72)"
        border="1px solid rgba(107,70,193,0.12)"
        borderRadius="16px"
        px={4}
        py={4}
        boxShadow="0 2px 10px rgba(15,12,41,0.06)"
      >
        <Text color="#4A5568" fontSize="sm">
          Sin reglas configuradas.
        </Text>
      </Box>
    );
  }

  return (
    <Box
      bg="rgba(255,255,255,0.72)"
      border="1px solid rgba(107,70,193,0.12)"
      borderRadius="18px"
      p={4}
      boxShadow="0 2px 12px rgba(15,12,41,0.07)"
      backdropFilter="blur(8px) saturate(150%)"
    >
      <VStack align="stretch" spacing={4}>
        <Box>
          <Text color="#4A5568" fontSize="sm" fontWeight="800" mb={2}>
            Que aplican
          </Text>

          {matched.length > 0 ? (
            <HStack spacing={2} flexWrap="wrap">
              {matched.map((outcome, index) => (
                <OutcomeTag
                  key={`matched-${outcome.id_outcome ?? outcome.id_level ?? index}`}
                  outcome={outcome}
                  fallback={`Outcome ${index + 1}`}
                />
              ))}
            </HStack>
          ) : (
            <Text color="#718096" fontSize="sm">
              Ningún outcome coincide con el puntaje actual.
            </Text>
          )}
        </Box>

        {all.length > 0 && (
          <Box>
            <Text color="#4A5568" fontSize="sm" fontWeight="800" mb={2}>
              Todas las reglas activas
            </Text>

            <HStack spacing={2} flexWrap="wrap">
              {all.map((outcome, index) => (
                <OutcomeTag
                  key={`all-${outcome.id_outcome ?? outcome.id_level ?? index}`}
                  outcome={outcome}
                  subtle
                  fallback={`Outcome ${index + 1}`}
                />
              ))}
            </HStack>
          </Box>
        )}
      </VStack>
    </Box>
  );
}