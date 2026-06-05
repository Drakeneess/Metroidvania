// src/assets/components/StudentTable.jsx

import {
  Box,
  Grid,
  GridItem,
  Text,
  Spinner,
  Tag,
  useBreakpointValue,
  Stack,
  VStack,
  HStack,
  Heading,
  Badge,
  Flex,
} from "@chakra-ui/react";

function formatDate(value) {
  if (!value) return "—";

  const date = new Date(value);

  if (Number.isNaN(date.getTime())) return "—";

  return date.toLocaleDateString("es-BO", {
    day: "2-digit",
    month: "2-digit",
    year: "numeric",
  });
}

function toNumber(value) {
  const number = Number(value);
  return Number.isFinite(number) ? number : 0;
}

function getStudentName(student) {
  return (
    student.full_name ||
    student.student_name ||
    student.name ||
    `Estudiante #${student.id_student}`
  );
}

function getSessionCount(student) {
  return toNumber(student.total_sessions);
}

function hasPlayed(student) {
  return getSessionCount(student) > 0;
}

function TableShell({
  title,
  description,
  count,
  badgeColor = "brand",
  children,
}) {
  const isCalm = badgeColor === "calm";

  return (
    <Box
      bg="rgba(35, 41, 70, 0.84)"
      border="1px solid"
      borderColor="soul.border"
      borderRadius="2xl"
      boxShadow="soul"
      backdropFilter="blur(18px)"
      overflow="hidden"
    >
      <Flex
        justify="space-between"
        align={{ base: "flex-start", md: "center" }}
        direction={{ base: "column", md: "row" }}
        gap={3}
        px={{ base: 4, md: 5 }}
        py={4}
        borderBottom="1px solid"
        borderColor="soul.softBorder"
      >
        <Box>
          <Heading size="sm" color="white">
            {title}
          </Heading>

          {description && (
            <Text color="gray.500" fontSize="sm" mt={1}>
              {description}
            </Text>
          )}
        </Box>

        <Badge
          px={3}
          py={1}
          borderRadius="full"
          bg={
            isCalm
              ? "rgba(0, 187, 249, 0.14)"
              : "rgba(169, 112, 255, 0.18)"
          }
          color={isCalm ? "calm" : "brand.100"}
          border="1px solid"
          borderColor={
            isCalm
              ? "rgba(0, 187, 249, 0.26)"
              : "rgba(169, 112, 255, 0.32)"
          }
        >
          {count} estudiantes
        </Badge>
      </Flex>

      {children}
    </Box>
  );
}

function StudentTableSection({
  title,
  description,
  students,
  onSelect,
  emptyText,
  badgeColor,
  showSessionColumns = false,
}) {
  const isMobile = useBreakpointValue({ base: true, md: false });

  if (!students.length) {
    return (
      <TableShell
        title={title}
        description={description}
        count={0}
        badgeColor={badgeColor}
      >
        <Box px={5} py={8} textAlign="center">
          <Text color="gray.400" fontWeight="semibold">
            {emptyText}
          </Text>
        </Box>
      </TableShell>
    );
  }

  const templateColumns = showSessionColumns
    ? "2fr 1fr 1fr 1fr 1.2fr"
    : "2fr 1fr 1fr 1.2fr";

  const minW = showSessionColumns ? "900px" : "760px";

  return (
    <TableShell
      title={title}
      description={description}
      count={students.length}
      badgeColor={badgeColor}
    >
      <Box overflowX="auto" w="100%">
        {!isMobile && (
          <Grid
            templateColumns={templateColumns}
            gap={0}
            minW={minW}
            px={5}
            py={3}
            bg="rgba(15, 12, 41, 0.45)"
            borderBottom="1px solid"
            borderColor="soul.softBorder"
            color="gray.400"
            fontWeight="bold"
            fontSize="xs"
            letterSpacing="0.08em"
            textTransform="uppercase"
          >
            <GridItem px={2}>Estudiante</GridItem>
            <GridItem px={2}>CI</GridItem>
            <GridItem px={2}>Edad</GridItem>

            {showSessionColumns ? (
              <>
                <GridItem px={2}>Partidas</GridItem>
                <GridItem px={2}>Última sesión</GridItem>
              </>
            ) : (
              <GridItem px={2}>Registro</GridItem>
            )}
          </Grid>
        )}

        {students.map((student, index) => {
          const hasAlert = Boolean(student.alert);
          const studentName = getStudentName(student);
          const sessionCount = getSessionCount(student);

          return (
            <Box
              key={student.id_student}
              bg={
                hasAlert
                  ? "rgba(239, 71, 111, 0.10)"
                  : index % 2 === 0
                    ? "rgba(255, 255, 255, 0.025)"
                    : "rgba(255, 255, 255, 0.045)"
              }
              borderBottom="1px solid"
              borderColor="soul.softBorder"
              _hover={{
                bg: "rgba(169, 112, 255, 0.12)",
              }}
              cursor={onSelect ? "pointer" : "default"}
              onClick={() => onSelect?.(student)}
              transition="background-color 0.2s ease"
              px={5}
              py={4}
            >
              {isMobile ? (
                <Stack spacing={2}>
                  <HStack spacing={2} flexWrap="wrap">
                    <Text fontWeight="bold" color="white" fontSize="md">
                      {studentName}
                    </Text>

                    {hasAlert && (
                      <Tag
                        size="sm"
                        bg={student.alert.color || "danger"}
                        color="white"
                        borderRadius="md"
                      >
                        {student.alert.type || "Alerta"}
                      </Tag>
                    )}
                  </HStack>

                  <Text fontSize="sm" color="gray.400">
                    ID:{" "}
                    <Text as="span" color="gray.200">
                      #{student.id_student}
                    </Text>
                  </Text>

                  <Text fontSize="sm" color="gray.400">
                    CI:{" "}
                    <Text as="span" color="gray.200">
                      {student.ci || "—"}
                    </Text>
                  </Text>

                  <Text fontSize="sm" color="gray.400">
                    Edad:{" "}
                    <Text as="span" color="gray.200">
                      {student.age_range || "—"}
                    </Text>
                  </Text>

                  {showSessionColumns ? (
                    <>
                      <Text fontSize="sm" color="gray.400">
                        Partidas:{" "}
                        <Text as="span" color="success" fontWeight="bold">
                          {sessionCount}
                        </Text>
                      </Text>

                      <Text fontSize="sm" color="gray.400">
                        Última sesión:{" "}
                        <Text as="span" color="gray.200">
                          {formatDate(student.last_session_date)}
                        </Text>
                      </Text>
                    </>
                  ) : (
                    <Text fontSize="sm" color="gray.400">
                      Registro:{" "}
                      <Text as="span" color="gray.200">
                        {formatDate(student.register_date)}
                      </Text>
                    </Text>
                  )}
                </Stack>
              ) : (
                <Grid
                  templateColumns={templateColumns}
                  gap={0}
                  minW={minW}
                  alignItems="center"
                  color="gray.200"
                >
                  <GridItem px={2}>
                    <HStack spacing={2}>
                      <Box minW={0}>
                        <Text noOfLines={1} fontWeight="semibold" color="white">
                          {studentName}
                        </Text>

                        <Text color="gray.500" fontSize="xs">
                          ID #{student.id_student}
                        </Text>
                      </Box>

                      {hasAlert && (
                        <Tag
                          size="sm"
                          bg={student.alert.color || "danger"}
                          color="white"
                          borderRadius="md"
                        >
                          {student.alert.type || "Alerta"}
                        </Tag>
                      )}
                    </HStack>
                  </GridItem>

                  <GridItem px={2}>
                    <Text color="gray.300">{student.ci || "—"}</Text>
                  </GridItem>

                  <GridItem px={2}>
                    <Text color="gray.300">
                      {student.age_range || "—"}
                    </Text>
                  </GridItem>

                  {showSessionColumns ? (
                    <>
                      <GridItem px={2}>
                        <Badge
                          px={2}
                          py={1}
                          borderRadius="md"
                          bg="rgba(6, 214, 160, 0.14)"
                          color="success"
                          border="1px solid"
                          borderColor="rgba(6, 214, 160, 0.26)"
                        >
                          {sessionCount}
                        </Badge>
                      </GridItem>

                      <GridItem px={2}>
                        <Text color="gray.300">
                          {formatDate(student.last_session_date)}
                        </Text>
                      </GridItem>
                    </>
                  ) : (
                    <GridItem px={2}>
                      <Text color="gray.300">
                        {formatDate(student.register_date)}
                      </Text>
                    </GridItem>
                  )}
                </Grid>
              )}
            </Box>
          );
        })}
      </Box>
    </TableShell>
  );
}

export default function StudentTable({
  students = [],
  loading = false,
  onSelect,
}) {
  if (loading) {
    return (
      <Flex
        minH="180px"
        align="center"
        justify="center"
        color="gray.300"
        border="1px dashed"
        borderColor="soul.border"
        borderRadius="2xl"
        bg="rgba(35, 41, 70, 0.54)"
      >
        <Spinner size="md" color="brand.200" mr={3} />
        <Text>Cargando estudiantes...</Text>
      </Flex>
    );
  }

  if (!students.length) {
    return (
      <Box
        p={6}
        border="1px dashed"
        borderColor="soul.border"
        borderRadius="2xl"
        bg="rgba(35, 41, 70, 0.54)"
        textAlign="center"
      >
        <Text color="gray.300" fontWeight="semibold">
          No hay estudiantes registrados.
        </Text>

        <Text color="gray.500" fontSize="sm" mt={1}>
          Cuando existan registros, aparecerán en esta sección.
        </Text>
      </Box>
    );
  }

  const studentsWithGames = students.filter(hasPlayed);

  const studentsOnlyRegistered = students.filter(
    (student) => !hasPlayed(student)
  );

  return (
    <VStack spacing={6} align="stretch">
      <StudentTableSection
        title="Estudiantes con partidas"
        description="Estudiantes que ya tienen playthroughs registrados en la API principal."
        students={studentsWithGames}
        onSelect={onSelect}
        emptyText="No hay estudiantes con partidas registradas."
        badgeColor="calm"
        showSessionColumns
      />

      <StudentTableSection
        title="Estudiantes solo registrados"
        description="Estudiantes creados en el sistema que todavía no tienen actividad de juego."
        students={studentsOnlyRegistered}
        onSelect={onSelect}
        emptyText="No hay estudiantes pendientes de iniciar actividad."
        badgeColor="brand"
      />
    </VStack>
  );
}