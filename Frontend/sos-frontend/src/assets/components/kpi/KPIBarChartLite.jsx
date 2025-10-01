import { Box, Heading, Text } from "@chakra-ui/react";

/**
 * data: Array<{ label: string, value: number }>
 */
export default function KPIBarChartLite({ title, data = [], loading = false, error = null }) {
  const width = 360;
  const height = 200;
  const padding = 24;

  const renderContent = () => {
    if (loading) return <Text fontSize="sm" color="gray.400">Cargando…</Text>;
    if (error) return <Text fontSize="sm" color="red.400">⚠ {error}</Text>;
    if (!Array.isArray(data) || data.length === 0) {
      return <Text fontSize="sm" color="gray.500">Sin datos en el rango seleccionado.</Text>;
    }

    const maxV = Math.max(...data.map(d => Number(d?.value) || 0), 1);
    const barW = (width - padding * 2) / data.length - 8;

    return (
      <svg width="100%" height={height} viewBox={`0 0 ${width} ${height}`}>
        {data.map((d, i) => {
          const v = Number(d?.value) || 0;
          const h = ((v / maxV) * (height - padding * 2));
          const x = padding + i * (barW + 8);
          const y = height - padding - h;
          return (
            <g key={i}>
              <rect x={x} y={y} width={barW} height={h} fill="teal" fillOpacity="0.7" rx="4" />
              <text x={x + barW / 2} y={height - 6} textAnchor="middle" fontSize="10" fill="currentColor">
                {d.label}
              </text>
              <text x={x + barW / 2} y={y - 6} textAnchor="middle" fontSize="10" fill="currentColor">
                {v}
              </text>
            </g>
          );
        })}
      </svg>
    );
  };

  return (
    <Box
      borderWidth="1px"
      borderRadius="2xl"
      p={5}
      minH="240px"
      bg="whiteAlpha.100"
      boxShadow="lg"
      backdropFilter="blur(6px)"
    >
      <Heading size="sm" mb={3} color="gray.200">{title}</Heading>
      <Box display="flex" alignItems="center" justifyContent="center" h="100%">
        {renderContent()}
      </Box>
    </Box>
  );
}
