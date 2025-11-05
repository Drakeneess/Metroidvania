export default function StudentSummary({ stats }) {
  return (
    <div
      style={{
        display: "grid",
        gap: 16,
        gridTemplateColumns: "repeat(auto-fit, minmax(180px, 1fr))",
        marginTop: 8,
      }}
    >
      <Card title="Puntaje total" value={stats.totalScore ?? 0} accent="linear-gradient(135deg, #805AD5, #6B46C1)" />
      <Card
        title="Avance"
        value={`${stats.answeredCount}/${stats.totalItems} (${stats.completion}%)`}
        accent="linear-gradient(135deg, #3182CE, #2B6CB0)"
      />
      <Card
        title="Última respuesta"
        value={
          stats.lastAnswerAt
            ? new Date(stats.lastAnswerAt).toLocaleString()
            : "—"
        }
        accent="linear-gradient(135deg, #38A169, #2F855A)"
      />
    </div>
  );
}

function Card({ title, value, accent }) {
  return (
    <div
      style={{
        borderRadius: 14,
        padding: "16px 18px",
        background: "rgba(255,255,255,0.7)",
        boxShadow: "0 4px 16px rgba(0,0,0,0.08)",
        backdropFilter: "blur(8px) saturate(160%)",
        border: "1px solid rgba(0,0,0,0.06)",
        transition: "transform 0.2s ease, box-shadow 0.3s ease",
        cursor: "default",
      }}
      onMouseEnter={(e) => {
        e.currentTarget.style.transform = "translateY(-3px)";
        e.currentTarget.style.boxShadow = "0 8px 20px rgba(0,0,0,0.12)";
      }}
      onMouseLeave={(e) => {
        e.currentTarget.style.transform = "translateY(0)";
        e.currentTarget.style.boxShadow = "0 4px 16px rgba(0,0,0,0.08)";
      }}
    >
      <div
        style={{
          fontSize: 13,
          color: "#2D3748",
          fontWeight: 600,
          marginBottom: 6,
          textTransform: "uppercase",
          letterSpacing: "0.3px",
        }}
      >
        {title}
      </div>

      <div
        style={{
          fontWeight: 800,
          fontSize: 18,
          background: accent,
          WebkitBackgroundClip: "text",
          WebkitTextFillColor: "transparent",
        }}
      >
        {value}
      </div>
    </div>
  );
}
