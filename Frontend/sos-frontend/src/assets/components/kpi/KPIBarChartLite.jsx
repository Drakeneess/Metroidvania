import {
  Box,
  Heading,
  Text,
  Spinner,
  Flex,
  HStack,
} from "@chakra-ui/react";

export default function KPIBarChartLite({
  title,
  data = [],
  loading = false,
  error = null,
  width = 420,
  height = 220,
  padding = 32,
}) {
  const clean = Array.isArray(data)
    ? data
        .map((d) => ({
          label: d?.label ?? "",
          value: Number(d?.value),
        }))
        .filter((d) => Number.isFinite(d.value))
    : [];

  const renderEmptyState = () => (
    <Flex
      minH={`${height}px`}
      align="center"
      justify="center"
      direction="column"
      gap={2}
      border="1px dashed"
      borderColor="soul.softBorder"
      borderRadius="xl"
      bg="rgba(255,255,255,0.025)"
    >
      {loading ? (
        <>
          <Spinner size="sm" color="brand.200" />
          <Text color="gray.400" fontSize="sm">
            Cargando datos...
          </Text>
        </>
      ) : error ? (
        <Text color="danger" fontSize="sm" fontWeight="semibold">
          ⚠ {error}
        </Text>
      ) : (
        <>
          <Text color="gray.300" fontWeight="semibold">
            Sin datos
          </Text>
          <Text color="gray.500" fontSize="sm">
            No hay registros para el rango seleccionado.
          </Text>
        </>
      )}
    </Flex>
  );

  if (!clean.length || loading || error) {
    return (
      <Box
        border="1px solid"
        borderColor="soul.border"
        borderRadius="2xl"
        p={5}
        bg="rgba(35, 41, 70, 0.84)"
        boxShadow="soul"
        backdropFilter="blur(18px)"
      >
        <Heading size="sm" color="gray.100" mb={4}>
          {title}
        </Heading>

        {renderEmptyState()}
      </Box>
    );
  }

  const maxValue = Math.max(...clean.map((d) => d.value), 1);
  const innerW = width - padding * 2;
  const innerH = height - padding * 2;

  const gap = 12;
  const barW = Math.max(16, innerW / clean.length - gap);

  const total = clean.reduce((sum, item) => sum + item.value, 0);

  return (
    <Box
      border="1px solid"
      borderColor="soul.border"
      borderRadius="2xl"
      p={5}
      bg="rgba(35, 41, 70, 0.84)"
      boxShadow="soul"
      backdropFilter="blur(18px)"
      transition="all 0.2s ease"
      _hover={{
        borderColor: "brand.300",
        boxShadow: "glow",
      }}
    >
      <Flex justify="space-between" align="center" mb={4} gap={4}>
        <Box>
          <Heading size="sm" color="gray.100">
            {title}
          </Heading>

          <Text color="gray.500" fontSize="xs" mt={1}>
            {clean.length} categorías registradas
          </Text>
        </Box>

        <HStack
          px={3}
          py={1}
          borderRadius="full"
          bg="rgba(0,187,249,0.12)"
          border="1px solid"
          borderColor="rgba(0,187,249,0.24)"
        >
          <Text color="calm" fontSize="xs" fontWeight="bold">
            Total: {total}
          </Text>
        </HStack>
      </Flex>

      <Box overflowX="auto">
        <svg
          width="100%"
          height={height}
          viewBox={`0 0 ${width} ${height}`}
          role="img"
          aria-label={title}
          style={{ minWidth: "360px" }}
        >
          {/* Grid */}
          {[0, 0.25, 0.5, 0.75, 1].map((tick) => {
            const y = padding + tick * innerH;

            return (
              <line
                key={tick}
                x1={padding}
                y1={y}
                x2={padding + innerW}
                y2={y}
                stroke="currentColor"
                strokeOpacity="0.08"
                strokeWidth="1"
              />
            );
          })}

          {/* Base */}
          <line
            x1={padding}
            y1={padding + innerH}
            x2={padding + innerW}
            y2={padding + innerH}
            stroke="currentColor"
            strokeOpacity="0.18"
            strokeWidth="1"
          />

          {clean.map((d, index) => {
            const value = d.value;
            const barH = (value / maxValue) * innerH;
            const x = padding + index * (barW + gap);
            const y = padding + innerH - barH;

            return (
              <g key={`${d.label}-${index}`}>
                <rect
                  x={x}
                  y={y}
                  width={barW}
                  height={barH}
                  rx="7"
                  fill="#A970FF"
                  fillOpacity="0.82"
                />

                <rect
                  x={x}
                  y={y}
                  width={barW}
                  height={barH}
                  rx="7"
                  fill="url(#barGlow)"
                  fillOpacity="0.35"
                />

                <text
                  x={x + barW / 2}
                  y={Math.max(14, y - 8)}
                  textAnchor="middle"
                  fontSize="11"
                  fontWeight="700"
                  fill="#F1E9FF"
                >
                  {value}
                </text>

                <text
                  x={x + barW / 2}
                  y={height - 8}
                  textAnchor="middle"
                  fontSize="10"
                  fill="#A0AEC0"
                >
                  {String(d.label).slice(0, 8)}
                </text>
              </g>
            );
          })}

          <defs>
            <linearGradient id="barGlow" x1="0" x2="0" y1="0" y2="1">
              <stop offset="0%" stopColor="#FFFFFF" stopOpacity="0.55" />
              <stop offset="100%" stopColor="#6B35C8" stopOpacity="0.05" />
            </linearGradient>
          </defs>
        </svg>
      </Box>
    </Box>
  );
}