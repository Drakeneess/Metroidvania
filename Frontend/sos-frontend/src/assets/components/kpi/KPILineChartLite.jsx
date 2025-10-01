import { Box, Text, Spinner } from "@chakra-ui/react";

export default function KPILineChartLite({
  title = "",
  data = [],         // [{ x, y }]
  loading = false,
  error = null,
  width = 360,
  height = 200,
  padding = 16,
}) {
  // 1) Saneamos entrada
  const raw = Array.isArray(data) ? data : [];
  let clean = raw
    .map((d) => ({
      x: d?.x ?? "",
      y: Number(d?.y),
    }))
    .filter((d) => Number.isFinite(d.y));

  // Si no hay datos válidos, placeholder
  if (!clean.length) {
    return (
      <Box borderWidth="1px" borderRadius="lg" p={4} bg="whiteAlpha.100">
        <Text mb={2} fontWeight="bold">{title}</Text>
        {loading ? (
          <Spinner size="sm" />
        ) : error ? (
          <Text color="red.400" fontSize="sm">⚠ {error}</Text>
        ) : (
          <Text color="gray.400" fontSize="sm">Sin datos</Text>
        )}
      </Box>
    );
  }

  // Si hay un único punto, duplicamos para evitar división por 0
  if (clean.length === 1) {
    clean = [clean[0], { ...clean[0] }];
  }

  // 2) Escalas
  const W = width;
  const H = height;
  const PW = Math.max(0, W - padding * 2);
  const PH = Math.max(0, H - padding * 2);

  const ys = clean.map((d) => d.y);
  const minY = Math.min(...ys, 0);
  const maxY = Math.max(...ys, 1);
  const rangeY = Math.max(1e-6, maxY - minY);

  const n = clean.length;
  const denom = Math.max(1, n - 1); // evita 0
  const xStep = PW / denom;

  const toPoint = (i, yVal) => {
    const x = padding + i * xStep;
    const yNorm = (yVal - minY) / rangeY; // 0..1
    const y = padding + (1 - yNorm) * PH;
    return `${x},${y}`;
  };

  const linePoints = clean.map((d, i) => toPoint(i, d.y)).join(" ");
  const firstX = padding + 0 * xStep;
  const lastX  = padding + (n - 1) * xStep;

  // Área: arranca en baseline izquierda, sube por la línea y vuelve al baseline derecha
  const areaPoints = [
    `${firstX},${padding + PH}`,
    ...clean.map((d, i) => toPoint(i, d.y)),
    `${lastX},${padding + PH}`,
  ].join(" ");

  return (
    <Box borderWidth="1px" borderRadius="lg" p={4} bg="whiteAlpha.100">
      <Text mb={2} fontWeight="bold">{title}</Text>
      {loading ? (
        <Spinner size="sm" />
      ) : error ? (
        <Text color="red.400" fontSize="sm">⚠ {error}</Text>
      ) : (
        <svg width={W} height={H} role="img" aria-label={title}>
          {/* Eje base */}
          <line
            x1={padding}
            y1={padding + PH}
            x2={padding + PW}
            y2={padding + PH}
            stroke="currentColor"
            strokeOpacity="0.2"
            strokeWidth="1"
          />
          {/* Área */}
          <polygon
            points={areaPoints}
            fill="currentColor"
            fillOpacity="0.12"
          />
          {/* Línea */}
          <polyline
            points={linePoints}
            fill="none"
            stroke="currentColor"
            strokeWidth="2"
            strokeLinecap="round"
            strokeLinejoin="round"
          />
        </svg>
      )}
    </Box>
  );
}
