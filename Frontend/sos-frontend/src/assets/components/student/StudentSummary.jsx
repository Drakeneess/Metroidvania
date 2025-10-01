export default function StudentSummary({ stats }) {
  return (
    <div style={{ display: "grid", gap: 16, gridTemplateColumns: "repeat(3,1fr)" }}>
      <Card title="Puntaje total" value={stats.totalScore ?? 0} />
      <Card
        title="Avance"
        value={`${stats.answeredCount}/${stats.totalItems} (${stats.completion}%)`}
      />
      <Card
        title="Última respuesta"
        value={stats.lastAnswerAt ? new Date(stats.lastAnswerAt).toLocaleString() : "—"}
      />
    </div>
  );
}

function Card({ title, value }) {
  return (
    <div style={{ border: "1px solid #E2E8F0", borderRadius: 12, padding: 16 }}>
      <div style={{ fontSize: 13, color: "#4A5568", marginBottom: 6 }}>{title}</div>
      <div style={{ fontWeight: 700 }}>{value}</div>
    </div>
  );
}
