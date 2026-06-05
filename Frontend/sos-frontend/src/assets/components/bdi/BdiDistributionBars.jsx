// src/components/bdi/BdiDistributionBars.jsx

import {
  Box,
  Flex,
  Text,
  VStack,
  Progress,
} from "@chakra-ui/react";

const scorePalette = {
  0: {
    label: "Mínimo",
    color: "#3182CE",
    gradient: "linear-gradient(90deg, #63B3ED, #3182CE)",
  },
  1: {
    label: "Leve",
    color: "#6B46C1",
    gradient: "linear-gradient(90deg, #B794F4, #6B46C1)",
  },
  2: {
    label: "Moderado",
    color: "#DD6B20",
    gradient: "linear-gradient(90deg, #F6AD55, #DD6B20)",
  },
  3: {
    label: "Alto",
    color: "#E53E3E",
    gradient: "linear-gradient(90deg, #FC8181, #E53E3E)",
  },
};

export default function BdiDistributionBars({ distribution = {}, total = 0 }) {
  const entries = Object.entries(distribution).sort(
    (a, b) => Number(a[0]) - Number(b[0])
  );

  if (!entries.length) {
    return (
      <Box
        color="#718096"
        fontSize="sm"
        bg="rgba(255,255,255,0.72)"
        border="1px solid rgba(107,70,193,0.12)"
        px={4}
        py={4}
        borderRadius="16px"
        boxShadow="0 2px 10px rgba(15,12,41,0.06)"
        textAlign="center"
      >
        Sin datos disponibles.
      </Box>
    );
  }

  return (
    <Box
      bg="rgba(255,255,255,0.72)"
      border="1px solid rgba(107,70,193,0.12)"
      p={4}
      borderRadius="18px"
      boxShadow="0 2px 12px rgba(15,12,41,0.07)"
      backdropFilter="blur(8px) saturate(150%)"
    >
      <VStack spacing={4} align="stretch">
        {entries.map(([score, count]) => {
          const numericScore = Number(score);
          const numericCount = Number(count) || 0;
          const pct = total > 0 ? Math.round((numericCount / total) * 100) : 0;

          const palette =
            scorePalette[numericScore] || {
              label: `Score ${score}`,
              color: "#6B46C1",
              gradient: "linear-gradient(90deg, #B794F4, #6B46C1)",
            };

          return (
            <Box key={score}>
              <Flex justify="space-between" align="center" mb={2} gap={3}>
                <Box>
                  <Text color="#1A202C" fontSize="sm" fontWeight="800">
                    Puntaje {score}
                  </Text>

                  <Text color="#718096" fontSize="xs" fontWeight="600">
                    {palette.label}
                  </Text>
                </Box>

                <Text color="#4A5568" fontSize="sm" fontWeight="700">
                  {numericCount} · {pct}%
                </Text>
              </Flex>

              <Progress
                value={pct}
                h="10px"
                borderRadius="full"
                bg="rgba(0,0,0,0.07)"
                sx={{
                  "& > div": {
                    background: palette.gradient,
                  },
                }}
              />
            </Box>
          );
        })}
      </VStack>
    </Box>
  );
}