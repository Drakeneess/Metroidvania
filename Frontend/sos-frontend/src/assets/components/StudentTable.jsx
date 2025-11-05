import { Box, Grid, GridItem, Text, Spinner, Tag, useBreakpointValue, Stack } from "@chakra-ui/react";

export default function StudentTable({ students = [], loading = false, onSelect }) {
  const borderClr = "gray.300";
  const headerBg = "gray.100";
  const textColor = "gray.800";
  const hoverBg = "blue.50";

  // 🔹 Saber si estamos en móvil o escritorio
  const isMobile = useBreakpointValue({ base: true, md: false });

  if (loading) return <Spinner size="lg" />;
  if (students.length === 0)
    return <Text color="gray.700">No hay estudiantes registrados.</Text>;

  return (
    <Box
      overflowX="auto"
      borderWidth="1px"
      borderColor={borderClr}
      borderRadius="lg"
      bg="white"
      w="100%"
    >
      {/* 💻 Cabecera solo en escritorio */}
      {!isMobile && (
        <Grid
          templateColumns="2fr 1fr 1fr 1fr"
          gap={0}
          bg={headerBg}
          color={textColor}
          borderBottom={`1px solid var(--chakra-colors-${borderClr.replace('.', '-')})`}
          minW="800px"
          px={4}
          py={2}
          fontWeight="semibold"
        >
          <GridItem px={2}>Nombre completo</GridItem>
          <GridItem px={2}>CI</GridItem>
          <GridItem px={2}>Edad</GridItem>
          <GridItem px={2}>Fecha de registro</GridItem>
        </Grid>
      )}

      {/* 🔹 Filas / Tarjetas */}
      {students.map((s, idx) => {
        const rowBg = idx % 2 === 0 ? "white" : "gray.50";
        const hasAlert = !!s.alert;
        const alertColor = s.alert?.color || "transparent";
        const alertBg = hasAlert ? `${alertColor}20` : rowBg;
        const alertTextClr = hasAlert ? textColor : "gray.600";

        return (
          <Box
            key={s.id_student}
            bg={alertBg}
            borderBottom={`1px solid var(--chakra-colors-${borderClr.replace('.', '-')})`}
            _hover={{ bg: hoverBg }}
            cursor={onSelect ? "pointer" : "default"}
            onClick={() => onSelect && onSelect(s)}
            transition="background-color 0.2s ease"
            px={4}
            py={3}
          >
            {isMobile ? (
              // 📱 Vista móvil
              <Stack spacing={1}>
                <Text fontWeight="bold" color={textColor} fontSize="md">
                  {s.full_name || "—"}
                  {hasAlert && (
                    <Tag
                      size="sm"
                      ml={2}
                      bg={s.alert.color}
                      color="white"
                      borderRadius="md"
                    >
                      {s.alert.type}
                    </Tag>
                  )}
                </Text>
                <Text fontSize="sm" color="gray.600">
                  CI: {s.ci || "—"}
                </Text>
                <Text fontSize="sm" color="gray.600">
                  Edad: {s.age_range || "—"}
                </Text>
                <Text fontSize="sm" color="gray.600">
                  Registro:{" "}
                  {s.register_date
                    ? new Date(s.register_date).toLocaleDateString()
                    : "—"}
                </Text>
              </Stack>
            ) : (
              // 💻 Vista escritorio
              <Grid
                templateColumns="2fr 1fr 1fr 1fr"
                gap={0}
                color={alertTextClr}
                minW="800px"
                alignItems="center"
              >
                <GridItem px={2}>
                  <Text noOfLines={1} fontWeight="medium">
                    {s.full_name}
                    {hasAlert && (
                      <Tag
                        size="sm"
                        ml={2}
                        bg={s.alert.color}
                        color="white"
                        borderRadius="md"
                      >
                        {s.alert.type}
                      </Tag>
                    )}
                  </Text>
                </GridItem>
                <GridItem px={2}>
                  <Text>{s.ci}</Text>
                </GridItem>
                <GridItem px={2}>
                  <Text>{s.age_range}</Text>
                </GridItem>
                <GridItem px={2}>
                  <Text>
                    {new Date(s.register_date).toLocaleDateString()}
                  </Text>
                </GridItem>
              </Grid>
            )}
          </Box>
        );
      })}
    </Box>
  );
}
