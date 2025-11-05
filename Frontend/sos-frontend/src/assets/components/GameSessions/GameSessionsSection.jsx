import GameSessionCard from "./GameSessionCard";
import GameSessionsTable from "./GameSessionsTable";
import GameSessionChart from "./GameSessionChart";
import { SectionTitle, Divider } from "../UIHelpers";

export default function GameSessionsSection({ sessions = [], error }) {
  // 🔸 Caso: Backend respondió 404 → nunca tuvo sesiones
  if (error === "NO_SESSIONS") {
    return (
      <p
        style={{
          color: "#6B46C1",
          fontSize: 14,
          background: "rgba(107,70,193,0.12)",
          padding: "12px 16px",
          borderRadius: 10,
          boxShadow: "0 2px 6px rgba(0,0,0,0.06)",
          textAlign: "center",
          fontWeight: 600,
        }}
      >
        El estudiante no tiene sesiones registradas todavía.
      </p>
    );
  }

  // 🔹 Caso: sin datos pero no por 404
  if (!sessions.length) {
    return (
      <p
        style={{
          color: "#A0AEC0",
          fontSize: 14,
          background: "rgba(255,255,255,0.6)",
          padding: "12px 16px",
          borderRadius: 10,
          boxShadow: "0 2px 6px rgba(0,0,0,0.05)",
          textAlign: "center",
        }}
      >
        Sin sesiones registradas.
      </p>
    );
  }

  // ✅ Caso: hay sesiones
  const sorted = [...sessions].sort(
    (a, b) => new Date(b.start_time) - new Date(a.start_time)
  );

  return (
    <div
      style={{
        display: "grid",
        gap: 24,
        background: "rgba(250,250,255,0.8)",
        borderRadius: 16,
        padding: 20,
        boxShadow: "0 4px 16px rgba(0,0,0,0.08)",
        backdropFilter: "blur(8px) saturate(160%)",
        color: "#1A202C",
      }}
    >
      <SectionTitle>Sesiones de juego</SectionTitle>

      {/* Últimas 3 sesiones */}
      <div style={{ display: "grid", gap: 12 }}>
        {sorted.slice(0, 3).map((s) => (
          <GameSessionCard key={s.id_session} session={s} />
        ))}
      </div>

      <Divider />

      <GameSessionChart sessions={sorted} />

      <Divider />

      <GameSessionsTable sessions={sorted} />
    </div>
  );
}
