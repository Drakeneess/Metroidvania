import { formatDateShort } from "../../../utils/formatDateShort";

export default function GameSessionsTable({ sessions = [] }) {
  console.group("🧩 DEBUG: GameSessionsTable");
  if (!sessions || !sessions.length) {
    console.warn("⚠️ No se recibieron sesiones o array vacío:", sessions);
  } else {
    console.table(
      sessions.map((s, i) => ({
        i,
        id_session: s.id_session,
        startTime: s.startTime,
        actions: s.actions,
        apm: s.apm,
        health: s.health,
        cluster: s.cluster,
      }))
    );
  }
  console.groupEnd();

  return (
    <div
      style={{
        borderRadius: 14,
        overflow: "hidden",
        background: "rgba(255,255,255,0.8)",
        boxShadow: "0 2px 8px rgba(0,0,0,0.05)",
        backdropFilter: "blur(8px) saturate(160%)",
      }}
    >
      {/* Header */}
      <div
        style={{
          background: "linear-gradient(135deg, #6B46C1 0%, #805AD5 100%)",
          color: "#fff",
          padding: "10px 14px",
          fontWeight: 600,
          display: "grid",
          gridTemplateColumns: "1fr 100px 100px 100px 80px",
          fontSize: 14,
        }}
      >
        <div>Fecha</div>
        <div style={{ textAlign: "right" }}>Acciones</div>
        <div style={{ textAlign: "right" }}>APM</div>
        <div style={{ textAlign: "right" }}>Salud</div>
        <div style={{ textAlign: "right" }}>Cluster</div>
      </div>

      {/* Rows */}
      {sessions.map((s, i) => {
        const healthPct = (s.health * 100).toFixed(0);
        const date = formatDateShort(s.start_time || s.startTime);

        return (
          <div
            key={s.id_session ?? i}
            style={{
              display: "grid",
              gridTemplateColumns: "1fr 100px 100px 100px 80px",
              padding: "10px 14px",
              borderBottom: "1px solid rgba(0,0,0,0.05)",
              fontSize: 13,
              color: "#2D3748",
              background:
                i % 2 === 0
                  ? "rgba(255,255,255,0.9)"
                  : "rgba(250,250,255,0.8)",
            }}
          >
            <div style={{ color: "#4A5568" }}>{date}</div>
            <div style={{ textAlign: "right" }}>{s.actions}</div>
            <div style={{ textAlign: "right" }}>{Math.round(s.apm)}</div>
            <div
              style={{
                textAlign: "right",
                fontWeight: 600,
                color:
                  s.health > 0.8
                    ? "#48BB78"
                    : s.health > 0.5
                    ? "#ECC94B"
                    : s.health > 0.3
                    ? "#ED8936"
                    : "#E53E3E",
              }}
            >
              {healthPct}%
            </div>
            <div
              style={{
                textAlign: "right",
                fontWeight: 700,
                color: "#6B46C1",
              }}
            >
              {s.cluster}
            </div>
          </div>
        );
      })}
    </div>
  );
}
