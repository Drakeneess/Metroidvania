// src/components/bdi/BdiDistributionBars.jsx
export default function BdiDistributionBars({ distribution = {}, total = 0 }) {
  const entries = Object.entries(distribution).sort(
    (a, b) => Number(a[0]) - Number(b[0])
  );

  if (!entries.length) {
    return <p style={{ color: "#718096" }}>Sin datos.</p>;
  }

  return (
    <div style={{ display: "flex", flexWrap: "wrap", gap: "16px" }}>
      {entries.map(([score, count]) => {
        const pct = total > 0 ? Math.round((count / total) * 100) : 0;
        return (
          <div key={score} style={{ minWidth: "220px", flex: "1" }}>
            <div
              style={{
                display: "flex",
                justifyContent: "space-between",
                marginBottom: "6px",
                fontSize: "14px",
              }}
            >
              <span>Score {score}</span>
              <span style={{ color: "#4A5568" }}>
                {count} ({pct}%)
              </span>
            </div>
            <div
              style={{
                height: "8px",
                background: "#EDF2F7",
                borderRadius: "6px",
                overflow: "hidden",
              }}
            >
              <div
                style={{
                  width: `${pct}%`,
                  height: "100%",
                  background: "#3182CE",
                  transition: "width 260ms ease",
                }}
              />
            </div>
          </div>
        );
      })}
    </div>
  );
}
