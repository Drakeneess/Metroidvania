// src/components/bdi/StudentResponsesTable.jsx

import {
  Badge,
  Box,
  Grid,
  GridItem,
  Text,
  useBreakpointValue,
  VStack,
  Stack,
} from "@chakra-ui/react";

const scoreToStyles = (score) => {
  const numericScore = Number(score);

  if (numericScore <= 0) {
    return {
      bg: "rgba(56, 161, 105, 0.12)",
      color: "#2F855A",
      border: "rgba(56, 161, 105, 0.28)",
    };
  }

  if (numericScore === 1) {
    return {
      bg: "rgba(214, 158, 46, 0.14)",
      color: "#B7791F",
      border: "rgba(214, 158, 46, 0.32)",
    };
  }

  if (numericScore === 2) {
    return {
      bg: "rgba(221, 107, 32, 0.14)",
      color: "#C05621",
      border: "rgba(221, 107, 32, 0.32)",
    };
  }

  return {
    bg: "rgba(229, 62, 62, 0.12)",
    color: "#C53030",
    border: "rgba(229, 62, 62, 0.30)",
  };
};

function ScoreBadge({ score, empty = false }) {
  if (empty) {
    return (
      <Badge
        px={2}
        py={1}
        borderRadius="md"
        bg="rgba(255,255,255,0.72)"
        color="#718096"
        border="1px solid rgba(0,0,0,0.08)"
      >
        —
      </Badge>
    );
  }

  const styles = scoreToStyles(score);

  return (
    <Badge
      px={2}
      py={1}
      borderRadius="md"
      bg={styles.bg}
      color={styles.color}
      border="1px solid"
      borderColor={styles.border}
      fontWeight="800"
    >
      {score ?? 0}
    </Badge>
  );
}

function MobileRow({ row, unanswered = false }) {
  return (
    <Box
      p={4}
      bg={unanswered ? "rgba(255,255,255,0.50)" : "rgba(255,255,255,0.78)"}
      border="1px solid rgba(107,70,193,0.10)"
      borderRadius="16px"
      boxShadow="0 1px 8px rgba(15,12,41,0.05)"
    >
      <Stack spacing={2}>
        <Text color="#1A202C" fontWeight="800">
          #{row.item_number} · {row.title || "—"}
        </Text>

        <Text color="#4A5568" fontSize="sm">
          <Text as="span" fontWeight="800">
            Original:
          </Text>{" "}
          {unanswered ? "Sin respuesta" : row.response?.response || "—"}
        </Text>

        <Text color="#4A5568" fontSize="sm" fontStyle="italic">
          <Text as="span" fontWeight="800" fontStyle="normal">
            Diálogo:
          </Text>{" "}
          {unanswered ? "—" : row.response?.response_symbol || "—"}
        </Text>

        <Box>
          <ScoreBadge score={row.response?.score} empty={unanswered} />
        </Box>
      </Stack>
    </Box>
  );
}

export default function StudentResponsesTable({
  answered = [],
  unanswered = [],
}) {
  const isMobile = useBreakpointValue({ base: true, md: false });

  if (!answered.length && !unanswered.length) {
    return (
      <Box
        p={5}
        bg="rgba(255,255,255,0.72)"
        border="1px dashed rgba(107,70,193,0.18)"
        borderRadius="18px"
        textAlign="center"
      >
        <Text color="#718096" fontWeight="600">
          No hay respuestas registradas.
        </Text>
      </Box>
    );
  }

  if (isMobile) {
    return (
      <VStack spacing={3} align="stretch">
        {answered.map((row) => (
          <MobileRow key={`ans-${row.id_item}`} row={row} />
        ))}

        {unanswered.length > 0 && (
          <Box
            px={3}
            py={2}
            bg="rgba(107,70,193,0.08)"
            borderRadius="12px"
          >
            <Text color="#6B46C1" fontWeight="800" fontSize="sm">
              Pendientes
            </Text>
          </Box>
        )}

        {unanswered.map((row) => (
          <MobileRow key={`un-${row.id_item}`} row={row} unanswered />
        ))}
      </VStack>
    );
  }

  return (
    <Box
      border="1px solid rgba(107,70,193,0.14)"
      borderRadius="18px"
      overflow="hidden"
      bg="rgba(255,255,255,0.76)"
      boxShadow="0 2px 12px rgba(15,12,41,0.07)"
      backdropFilter="blur(8px) saturate(150%)"
    >
      <Box overflowX="auto">
        <Grid
          templateColumns="60px 1.6fr 1.8fr 1.8fr 90px"
          minW="900px"
          gap={0}
          bg="rgba(107,70,193,0.08)"
          color="#2D3748"
          borderBottom="1px solid rgba(107,70,193,0.12)"
          px={4}
          py={3}
          fontWeight="800"
          fontSize="xs"
          letterSpacing="0.06em"
          textTransform="uppercase"
        >
          <GridItem>#</GridItem>
          <GridItem>Ítem</GridItem>
          <GridItem>Original BDI</GridItem>
          <GridItem>Diálogo simbólico</GridItem>
          <GridItem textAlign="right">Score</GridItem>
        </Grid>

        {answered.map((row, index) => (
          <Grid
            key={`ans-${row.id_item}`}
            templateColumns="60px 1.6fr 1.8fr 1.8fr 90px"
            minW="900px"
            gap={0}
            borderBottom="1px solid rgba(107,70,193,0.08)"
            px={4}
            py={3}
            bg={index % 2 === 0 ? "rgba(255,255,255,0.72)" : "rgba(248,247,255,0.72)"}
            color="#2D3748"
            alignItems="center"
            _hover={{
              bg: "rgba(237,233,254,0.88)",
            }}
          >
            <GridItem>
              <Text fontWeight="800" color="#6B46C1">
                {row.item_number}
              </Text>
            </GridItem>

            <GridItem pr={3}>
              <Text noOfLines={2} title={row.title || ""} fontWeight="700">
                {row.title || "—"}
              </Text>
            </GridItem>

            <GridItem pr={3}>
              <Text noOfLines={2} title={row.response?.response || ""}>
                {row.response?.response || "—"}
              </Text>
            </GridItem>

            <GridItem pr={3}>
              <Text
                noOfLines={2}
                title={row.response?.response_symbol || ""}
                fontStyle={row.response?.response_symbol ? "italic" : "normal"}
                color={row.response?.response_symbol ? "#4A5568" : "#718096"}
              >
                {row.response?.response_symbol || "—"}
              </Text>
            </GridItem>

            <GridItem textAlign="right">
              <ScoreBadge score={row.response?.score} />
            </GridItem>
          </Grid>
        ))}

        {unanswered.length > 0 && (
          <Box
            bg="rgba(107,70,193,0.08)"
            px={4}
            py={3}
            fontWeight="800"
            color="#6B46C1"
            borderTop="1px solid rgba(107,70,193,0.12)"
          >
            Pendientes
          </Box>
        )}

        {unanswered.map((row, index) => (
          <Grid
            key={`un-${row.id_item}`}
            templateColumns="60px 1.6fr 1.8fr 1.8fr 90px"
            minW="900px"
            gap={0}
            borderTop="1px solid rgba(107,70,193,0.08)"
            px={4}
            py={3}
            bg={index % 2 === 0 ? "rgba(255,255,255,0.55)" : "rgba(248,247,255,0.62)"}
            color="#4A5568"
            alignItems="center"
            opacity={0.92}
          >
            <GridItem>
              <Text fontWeight="800" color="#805AD5">
                {row.item_number}
              </Text>
            </GridItem>

            <GridItem pr={3}>
              <Text noOfLines={2}>{row.title || "—"}</Text>
            </GridItem>

            <GridItem pr={3}>
              <Text color="#718096">Sin respuesta</Text>
            </GridItem>

            <GridItem pr={3}>
              <Text color="#A0AEC0">—</Text>
            </GridItem>

            <GridItem textAlign="right">
              <ScoreBadge empty />
            </GridItem>
          </Grid>
        ))}
      </Box>
    </Box>
  );
}