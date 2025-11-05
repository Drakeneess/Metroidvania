export default function GameSessionCard({ session }) {
  const durationMin = Math.round((session.actions || 0) / session.apm) || 1;

  return (
    <div
      style={{
        borderRadius: 14,
        padding: "14px 18px",
        display: "flex",
        justifyContent: "space-between",
        alignItems: "center",
        background: "rgba(255,255,255,0.75)",
        boxShadow: "0 2px 8px rgba(0,0,0,0.05)",
        border: "1px solid rgba(0,0,0,0.06)",
        transition: "transform 0.2s ease, box-shadow 0.3s ease",
      }}
      onMouseEnter={(e) => {
        e.currentTarget.style.transform = "translateY(-2px)";
        e.currentTarget.style.boxShadow = "0 6px 14px rgba(0,0,0,0.12)";
      }}
      onMouseLeave={(e) => {
        e.currentTarget.style.transform = "translateY(0)";
        e.currentTarget.style.boxShadow = "0 2px 8px rgba(0,0,0,0.05)";
      }}
    >
      <div>
        <strong style={{ color: "#1A202C" }}>
          Sesión #{session.id_session}
        </strong>
        <div style={{ fontSize: 13, color: "#4A5568" }}>
          Acciones: {session.actions} · APM: {Math.round(session.apm)} ·{" "}
          Duración: {durationMin}m
        </div>
      </div>
      <div style={{ textAlign: "right" }}>
        <div style={{ fontSize: 12, color: "#718096" }}>Cluster</div>
        <div
          style={{
            fontWeight: 700,
            fontSize: 16,
            background: "linear-gradient(135deg, #805AD5, #6B46C1)",
            WebkitBackgroundClip: "text",
            WebkitTextFillColor: "transparent",
          }}
        >
          {session.cluster}
        </div>
      </div>
    </div>
  );
}
