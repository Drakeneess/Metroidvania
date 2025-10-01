import { Box, Grid, GridItem, Text, Spinner } from "@chakra-ui/react";

export default function StudentTable({ students = [], loading = false, onSelect }) {
  const borderClr = "gray.300";
  const headerBg = "gray.100";
  const textColor = "gray.800";
  const hoverBg = "blue.50";

  if (loading) return <Spinner size="lg" />;
  if (students.length === 0) return <Text color="gray.700">No hay estudiantes registrados.</Text>;

  return (
    <Box overflowX="auto" borderWidth="1px" borderColor={borderClr} borderRadius="lg" bg="white">
      <Grid
        templateColumns="2fr 1fr 1fr 1fr"
        gap={0}
        bg={headerBg}
        color={textColor}
        borderBottom={`1px solid var(--chakra-colors-${borderClr.replace('.', '-')})`}
        minW="700px"
        px={4}
        py={2}
        fontWeight="semibold"
      >
        <GridItem px={2}>Nombre completo</GridItem>
        <GridItem px={2}>CI</GridItem>
        <GridItem px={2}>Edad</GridItem>
        <GridItem px={2}>Fecha de registro</GridItem>
      </Grid>

      {students.map((s, idx) => {
        const rowBg = idx % 2 === 0 ? "white" : "gray.50";
        return (
          <Grid
            key={s.id_student}
            templateColumns="2fr 1fr 1fr 1fr"
            gap={0}
            borderBottom={`1px solid var(--chakra-colors-${borderClr.replace('.', '-')})`}
            minW="700px"
            px={4}
            py={2}
            bg={rowBg}
            color={textColor}
            _hover={{ bg: hoverBg }}
            cursor={onSelect ? "pointer" : "default"}
            onClick={() => onSelect && onSelect(s)}
          >
            <GridItem px={2}>
              <Text noOfLines={1}>{s.full_name}</Text>
            </GridItem>
            <GridItem px={2}><Text>{s.ci}</Text></GridItem>
            <GridItem px={2}><Text>{s.age_range}</Text></GridItem>
            <GridItem px={2}>
              <Text>{new Date(s.register_date).toLocaleDateString()}</Text>
            </GridItem>
          </Grid>
        );
      })}
    </Box>
  );
}
