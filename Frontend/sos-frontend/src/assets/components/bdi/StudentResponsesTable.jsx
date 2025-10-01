import { Box, Grid, GridItem, Text, Badge } from "@chakra-ui/react";

const scoreToScheme = (s) => {
  const n = Number(s);
  if (n <= 0) return "green";
  if (n === 1) return "yellow";
  if (n === 2) return "orange";
  return "red";
};

export default function StudentResponsesTable({ answered = [], unanswered = [] }) {
  const borderClr = "gray.300";
  const headerBg = "gray.100";
  const textColor = "gray.800";

  return (
    <Box borderWidth="1px" borderColor={borderClr} borderRadius="lg" overflow="hidden" bg="white">
      {/* +1 columna: Original | +1 columna: Diálogo */}
      <Grid
        templateColumns="60px 2fr 2fr 2fr 80px"
        gap={0}
        bg={headerBg}
        color={textColor}
        borderBottom={`1px solid var(--chakra-colors-${borderClr.replace('.', '-')})`}
        px={4}
        py={2}
        fontWeight="semibold"
      >
        <GridItem>#</GridItem>
        <GridItem>Ítem</GridItem>
        <GridItem>Original (BDI)</GridItem>
        <GridItem>Diálogo (simbólico)</GridItem>
        <GridItem textAlign="right">Score</GridItem>
      </Grid>

      {answered.map((row, idx) => (
        <Grid
          key={`ans-${row.id_item}`}
          templateColumns="60px 2fr 2fr 2fr 80px"
          gap={0}
          borderBottom={`1px solid var(--chakra-colors-${borderClr.replace('.', '-')})`}
          px={4}
          py={2}
          bg={idx % 2 === 0 ? "white" : "gray.50"}
          color={textColor}
        >
          <GridItem><Text>{row.item_number}</Text></GridItem>

          {/* Ítem */}
          <GridItem pr={2}>
            <Text noOfLines={2} title={row.title || ""}>
              {row.title || "—"}
            </Text>
          </GridItem>

          {/* Original */}
          <GridItem pr={2}>
            <Text noOfLines={2} title={row.response?.response || ""}>
              {row.response?.response || "—"}
            </Text>
          </GridItem>

          {/* Diálogo simbólico */}
          <GridItem pr={2}>
            <Text
              noOfLines={2}
              title={row.response?.response_symbol || ""}
              fontStyle={row.response?.response_symbol ? "italic" : "normal"}
              color={row.response?.response_symbol ? "gray.700" : "gray.600"}
            >
              {row.response?.response_symbol || "—"}
            </Text>
          </GridItem>

          {/* Score */}
          <GridItem textAlign="right">
            <Badge colorScheme={scoreToScheme(row.response?.score)} variant="solid">
              {row.response?.score ?? 0}
            </Badge>
          </GridItem>
        </Grid>
      ))}

      {unanswered.length > 0 && (
        <>
          <Box bg="gray.100" px={4} py={2} fontWeight="semibold" color={textColor}>
            Pendientes
          </Box>
          {unanswered.map((row, idx) => (
            <Grid
              key={`un-${row.id_item}`}
              templateColumns="60px 2fr 2fr 2fr 80px"
              gap={0}
              borderTop={`1px solid var(--chakra-colors-${borderClr.replace('.', '-')})`}
              px={4}
              py={2}
              opacity={0.9}
              bg={idx % 2 === 0 ? "white" : "gray.50"}
              color={textColor}
            >
              <GridItem><Text>{row.item_number}</Text></GridItem>
              <GridItem pr={2}><Text noOfLines={2}>{row.title}</Text></GridItem>
              <GridItem pr={2}><Text color="gray.600">Sin respuesta</Text></GridItem>
              <GridItem pr={2}><Text color="gray.600">—</Text></GridItem>
              <GridItem textAlign="right"><Badge variant="outline">—</Badge></GridItem>
            </Grid>
          ))}
        </>
      )}
    </Box>
  );
}
