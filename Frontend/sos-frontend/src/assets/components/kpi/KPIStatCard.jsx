import { Box, Flex, Text, Spinner } from "@chakra-ui/react";

export default function KPIStatCard({
  label,
  value,
  help,
  unit,               // opcional: "min", "seg", etc.
  loading = false,
  error = null,
  format = "int",     // "int" | "float"
}) {
  const formatNumber = (v) => {
    if (v == null || Number.isNaN(v)) return "—";
    if (format === "float") return new Intl.NumberFormat("es-ES", { maximumFractionDigits: 1 }).format(v);
    return new Intl.NumberFormat("es-ES").format(v);
  };

  return (
    <Box
      borderWidth="1px"
      borderRadius="2xl"
      p={6}
      bg="whiteAlpha.100"
      boxShadow="lg"
      backdropFilter="blur(6px)"
    >
      <Flex direction="column" gap={1}>
        <Text color="gray.300" fontWeight="semibold" fontSize="sm">{label}</Text>

        {loading ? (
          <Spinner size="sm" />
        ) : error ? (
          <Text color="red.400" fontSize="sm">⚠ {error}</Text>
        ) : (
          <Flex align="baseline" gap={2}>
            <Text fontSize="3xl" fontWeight="bold">
              {formatNumber(value)}
            </Text>
            {unit ? <Text fontSize="sm" color="gray.400">{unit}</Text> : null}
          </Flex>
        )}

        {help && !loading && !error ? (
          <Text fontSize="xs" color="gray.400">{help}</Text>
        ) : null}
      </Flex>
    </Box>
  );
}
