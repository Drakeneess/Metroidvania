// src/components/GameSessions/GameSessionChart.jsx

import { Box, Text } from "@chakra-ui/react";
import {
  LineChart,
  Line,
  XAxis,
  YAxis,
  Tooltip,
  ResponsiveContainer,
  CartesianGrid,
} from "recharts";
import { formatDateShort } from "../../../utils/formatDateShort";

function safeNumber(value, fallback = 0) {
  const number = Number(value);
  return Number.isFinite(number) ? number : fallback;
}

function getHealthPercent(value) {
  const health = safeNumber(value);

  if (health <= 1) return Math.round(health * 100);

  return Math.round(health);
}

export default function GameSessionChart({ sessions = [] }) {
  const data = sessions.map((session) => ({
    name: formatDateShort(session.start_time || session.startTime),
    apm: Math.round(safeNumber(session.apm)),
    health: getHealthPercent(session.health),
  }));

  if (!data.length) {
    return (
      <Box
        color="#718096"
        fontSize="sm"
        bg="rgba(255,255,255,0.72)"
        border="1px solid rgba(107,70,193,0.12)"
        px={4}
        py={4}
        borderRadius="16px"
        boxShadow="0 2px 10px rgba(15,12,41,0.06)"
        textAlign="center"
      >
        Sin datos para graficar.
      </Box>
    );
  }

  return (
    <Box
      w="100%"
      h="280px"
      bg="rgba(255,255,255,0.76)"
      border="1px solid rgba(107,70,193,0.13)"
      borderRadius="18px"
      p={4}
      boxShadow="0 2px 12px rgba(15,12,41,0.07)"
      backdropFilter="blur(8px) saturate(150%)"
    >
      <Text color="#1A202C" fontWeight="900" fontSize="sm" mb={3}>
        Evolución de sesiones
      </Text>

      <ResponsiveContainer width="100%" height="88%">
        <LineChart data={data}>
          <CartesianGrid strokeDasharray="3 3" stroke="rgba(107,70,193,0.12)" />

          <XAxis
            dataKey="name"
            tick={{ fill: "#4A5568", fontSize: 12, fontWeight: 600 }}
            axisLine={{ stroke: "rgba(107,70,193,0.18)" }}
            tickLine={{ stroke: "rgba(107,70,193,0.18)" }}
          />

          <YAxis
            tick={{ fill: "#4A5568", fontSize: 12, fontWeight: 600 }}
            axisLine={{ stroke: "rgba(107,70,193,0.18)" }}
            tickLine={{ stroke: "rgba(107,70,193,0.18)" }}
          />

          <Tooltip
            contentStyle={{
              background: "rgba(255,255,255,0.96)",
              borderRadius: 12,
              border: "1px solid rgba(107,70,193,0.16)",
              boxShadow: "0 8px 24px rgba(15,12,41,0.13)",
            }}
            labelStyle={{
              color: "#2D3748",
              fontWeight: 800,
            }}
          />

          <Line
            type="monotone"
            dataKey="apm"
            stroke="#6B46C1"
            name="APM"
            strokeWidth={3}
            dot={{ r: 3, strokeWidth: 2 }}
            activeDot={{ r: 5 }}
          />

          <Line
            type="monotone"
            dataKey="health"
            stroke="#38A169"
            name="Salud (%)"
            strokeWidth={3}
            dot={{ r: 3, strokeWidth: 2 }}
            activeDot={{ r: 5 }}
          />
        </LineChart>
      </ResponsiveContainer>
    </Box>
  );
}