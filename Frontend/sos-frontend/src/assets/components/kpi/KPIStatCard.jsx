import {
  Box,
  Flex,
  Text,
  Spinner,
  Skeleton,
  HStack,
} from "@chakra-ui/react";

export default function KPIStatCard({
  label,
  value,
  help,
  unit,
  loading = false,
  error = null,
  format = "int",
}) {
  const formatNumber = (v) => {
    if (v == null || Number.isNaN(v)) return "—";

    if (format === "float") {
      return new Intl.NumberFormat("es-BO", {
        maximumFractionDigits: 1,
      }).format(v);
    }

    return new Intl.NumberFormat("es-BO").format(v);
  };

  return (
    <Box
      position="relative"
      overflow="hidden"
      border="1px solid"
      borderColor="soul.border"
      borderRadius="2xl"
      p={6}
      minH="150px"
      bg="rgba(35, 41, 70, 0.84)"
      boxShadow="soul"
      backdropFilter="blur(18px)"
      transition="all 0.2s ease"
      _hover={{
        transform: "translateY(-2px)",
        borderColor: "brand.300",
        boxShadow: "glow",
      }}
      _before={{
        content: '""',
        position: "absolute",
        inset: 0,
        bg: "radial-gradient(circle at top right, rgba(169,112,255,0.18), transparent 38%)",
        pointerEvents: "none",
      }}
    >
      <Flex position="relative" direction="column" gap={2}>
        <Text
          color="gray.400"
          fontWeight="semibold"
          fontSize="sm"
          letterSpacing="0.02em"
        >
          {label}
        </Text>

        {loading ? (
          <Flex align="center" minH="48px">
            <Spinner size="sm" color="brand.200" mr={3} />
            <Skeleton height="28px" width="90px" borderRadius="md" />
          </Flex>
        ) : error ? (
          <Text color="danger" fontSize="sm" fontWeight="semibold">
            ⚠ {error}
          </Text>
        ) : (
          <HStack align="baseline" spacing={2}>
            <Text
              fontSize={{ base: "2xl", md: "3xl" }}
              fontWeight="800"
              color="white"
              letterSpacing="-0.04em"
              lineHeight="1"
            >
              {formatNumber(value)}
            </Text>

            {unit ? (
              <Text fontSize="sm" color="brand.200" fontWeight="semibold">
                {unit}
              </Text>
            ) : null}
          </HStack>
        )}

        {help && !loading && !error ? (
          <Text fontSize="xs" color="gray.500" mt={1}>
            {help}
          </Text>
        ) : null}
      </Flex>
    </Box>
  );
}