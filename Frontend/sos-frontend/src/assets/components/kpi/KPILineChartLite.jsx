import {
  Box,
  Text,
  Spinner,
  Flex,
  Heading,
  HStack,
} from "@chakra-ui/react";

export default function KPILineChartLite({
  title = "",
  data = [],
  loading = false,
  error = null,
  width = 420,
  height = 220,
  padding = 28,
}) {
  const raw = Array.isArray(data) ? data : [];

  let clean = raw
    .map((d) => ({
      x: d?.x ?? "",
      y: Number(d?.y),
    }))
    .filter((d) => Number.isFinite(d.y));

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

  if (clean.length === 1) {
    clean = [clean[0], { ...clean[0] }];
  }

  const W = width;
  const H = height;
  const PW = Math.max(0, W - padding * 2);
  const PH = Math.max(0, H - padding * 2);

  const ys = clean.map((d) => d.y);
  const minY = Math.min(...ys, 0);
  const maxY = Math.max(...ys, 1);
  const rangeY = Math.max(1e-6, maxY - minY);

  const n = clean.length;
  const denom = Math.max(1, n - 1);
  const xStep = PW / denom;

  const getPoint = (i, yVal) => {
    const x = padding + i * xStep;
    const yNorm = (yVal - minY) / rangeY;
    const y = padding + (1 - yNorm) * PH;

    return { x, y };
  };

  const points = clean.map((d, i) => getPoint(i, d.y));

  const linePoints = points.map((p) => `${p.x},${p.y}`).join(" ");

  const areaPoints = [
    `${points[0].x},${padding + PH}`,
    ...points.map((p) => `${p.x},${p.y}`),
    `${points[points.length - 1].x},${padding + PH}`,
  ].join(" ");

  const lastValue = clean[clean.length - 1]?.y ?? 0;

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
            {clean.length} puntos registrados
          </Text>
        </Box>

        <HStack
          px={3}
          py={1}
          borderRadius="full"
          bg="rgba(169,112,255,0.14)"
          border="1px solid"
          borderColor="rgba(169,112,255,0.26)"
        >
          <Text color="brand.100" fontSize="xs" fontWeight="bold">
            Último: {lastValue}
          </Text>
        </HStack>
      </Flex>

      <Box overflowX="auto">
        <svg
          width="100%"
          height={H}
          viewBox={`0 0 ${W} ${H}`}
          role="img"
          aria-label={title}
          style={{ minWidth: "360px" }}
        >
          {/* Grid horizontal */}
          {[0, 0.25, 0.5, 0.75, 1].map((tick) => {
            const y = padding + tick * PH;

            return (
              <line
                key={tick}
                x1={padding}
                y1={y}
                x2={padding + PW}
                y2={y}
                stroke="currentColor"
                strokeOpacity="0.08"
                strokeWidth="1"
              />
            );
          })}

          {/* Área */}
          <polygon
            points={areaPoints}
            fill="#A970FF"
            fillOpacity="0.14"
          />

          {/* Línea */}
          <polyline
            points={linePoints}
            fill="none"
            stroke="#A970FF"
            strokeWidth="3"
            strokeLinecap="round"
            strokeLinejoin="round"
          />

          {/* Puntos */}
          {points.map((p, index) => (
            <circle
              key={index}
              cx={p.x}
              cy={p.y}
              r="4"
              fill="#C49BFF"
              stroke="#232946"
              strokeWidth="2"
            />
          ))}

          {/* Eje base */}
          <line
            x1={padding}
            y1={padding + PH}
            x2={padding + PW}
            y2={padding + PH}
            stroke="currentColor"
            strokeOpacity="0.18"
            strokeWidth="1"
          />
        </svg>
      </Box>
    </Box>
  );
}