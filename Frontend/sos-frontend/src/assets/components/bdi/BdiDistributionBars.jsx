export default function BdiDistributionBars({ distribution = {}, total = 0 }) {
  const entries = Object.entries(distribution).sort(
    (a, b) => Number(a[0]) - Number(b[0])
  );

  if (!entries.length) {
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
        Sin datos disponibles.
      </p>
    );
  }

  return (
    <div
      style={{
        display: "flex",
        flexWrap: "wrap",
        gap: 20,
        background: "rgba(255,255,255,0.6)",
        padding: 16,
        borderRadius: 14,
        boxShadow: "0 2px 8px rgba(0,0,0,0.05)",
        backdropFilter: "blur(6px) saturate(150%)",
      }}
    >
      {entries.map(([score, count], i) => {
        const pct = total > 0 ? Math.round((count / total) * 100) : 0;

        // paleta emocional (gradientes suaves por score)
        const gradients = [
          "linear-gradient(90deg, #63B3ED, #3182CE)", // azul
          "linear-gradient(90deg, #9F7AEA, #6B46C1)", // violeta
          "linear-gradient(90deg, #F6AD55, #DD6B20)", // naranja
          "linear-gradient(90deg, #FC8181, #E53E3E)", // rojo
          "linear-gradient(90deg, #48BB78, #2F855A)", // verde
        ];
        const barColor = gradients[i % gradients.length];

        return (
          <div key={score} style={{ minWidth: "240px", flex: "1" }}>
            <div
              style={{
                display: "flex",
                justifyContent: "space-between",
                marginBottom: 6,
                fontSize: 14,
                fontWeight: 600,
                color: "#2D3748",
              }}
            >
              <span>Puntaje {score}</span>
              <span style={{ color: "#4A5568", fontWeight: 500 }}>
                {count} ({pct}%)
              </span>
            </div>
            <div
              style={{
                height: "10px",
                background: "rgba(0,0,0,0.08)",
                borderRadius: "6px",
                overflow: "hidden",
                boxShadow: "inset 0 1px 3px rgba(0,0,0,0.15)",
              }}
            >
              <div
                style={{
                  width: `${pct}%`,
                  height: "100%",
                  background: barColor,
                  transition: "width 300ms ease",
                }}
              />
            </div>
          </div>
        );
      })}
    </div>
  );
}
