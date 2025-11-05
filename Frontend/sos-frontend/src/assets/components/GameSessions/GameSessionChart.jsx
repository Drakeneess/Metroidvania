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

export default function GameSessionChart({ sessions = [] }) {
  const data = sessions.map((s) => ({
    name: formatDateShort(s.start_time || s.startTime),
    apm: s.apm || 0,
    health: s.health ? Math.round(s.health * 100) : 0,
  }));

  if (!data.length)
    return (
      <p
        style={{
          color: "#A0AEC0",
          fontSize: 14,
          background: "rgba(255,255,255,0.6)",
          padding: "12px 16px",
          borderRadius: 10,
          textAlign: "center",
        }}
      >
        Sin datos para graficar.
      </p>
    );

  return (
    <div
      style={{
        width: "100%",
        height: 260,
        background: "rgba(255,255,255,0.8)",
        borderRadius: 14,
        padding: 12,
        boxShadow: "0 2px 8px rgba(0,0,0,0.06)",
        backdropFilter: "blur(8px) saturate(160%)",
      }}
    >
      <ResponsiveContainer>
        <LineChart data={data}>
          <CartesianGrid strokeDasharray="3 3" stroke="#E2E8F0" />
          <XAxis dataKey="name" tick={{ fill: "#4A5568", fontSize: 12 }} />
          <YAxis tick={{ fill: "#4A5568", fontSize: 12 }} />
          <Tooltip
            contentStyle={{
              background: "rgba(255,255,255,0.9)",
              borderRadius: 8,
              border: "1px solid rgba(0,0,0,0.1)",
            }}
            labelStyle={{ color: "#2D3748", fontWeight: 600 }}
          />
          <Line
            type="monotone"
            dataKey="apm"
            stroke="#6B46C1"
            name="APM"
            strokeWidth={2}
            dot={false}
          />
          <Line
            type="monotone"
            dataKey="health"
            stroke="#48BB78"
            name="Salud (%)"
            strokeWidth={2}
            dot={false}
          />
        </LineChart>
      </ResponsiveContainer>
    </div>
  );
}
