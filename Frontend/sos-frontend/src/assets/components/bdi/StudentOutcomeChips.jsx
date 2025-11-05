// src/components/bdi/StudentOutcomeChips.jsx
export default function StudentOutcomeChips({ matched = [], all = [] }) {
  // 🎨 Conversión de nivel de severidad a color
  const severityToColor = (sev) => {
    switch ((sev || "").toLowerCase()) {
      case "mild":
        return "#38A169"; // verde
      case "moderate":
        return "#D69E2E"; // amarillo
      case "severe":
        return "#E53E3E"; // rojo
      default:
        return "#3182CE"; // azul
    }
  };

  // 🧩 Componente de chip individual
  const Chip = ({ text, color, subtle, title }) => (
    <span
      title={title || ""}
      style={{
        display: "inline-block",
        padding: "4px 10px",
        borderRadius: 999,
        background: subtle ? "#fff" : color,
        border: subtle ? `1px solid ${color}` : "none",
        color: subtle ? "#2D3748" : "#fff",
        fontSize: 13,
        marginRight: 6,
        marginBottom: 6,
        cursor: title ? "help" : "default",
        transition: "background 0.2s ease"
      }}
    >
      {text}
    </span>
  );

  // 🧠 Si no hay reglas configuradas
  if (!all.length && !matched.length) {
    return <p style={{ color: "#4A5568" }}>Sin reglas configuradas.</p>;
  }

  // 🧾 Renderizado principal
  return (
    <div>
      {/* Outcomes que aplican */}
      {matched.length > 0 ? (
        <>
          <div style={{ fontSize: 13, color: "#4A5568", marginBottom: 6 }}>
            Que aplican:
          </div>
          <div>
            {matched.map((o, i) => (
              <Chip
                key={`matched-${o.id_outcome ?? i}`} // ✅ key siempre única
                text={`${o.name}${o.narrative_flag ? ` · ${o.narrative_flag}` : ""}`}
                color={severityToColor(o.severity_level)}
                title={o.description || ""}
              />
            ))}
          </div>
        </>
      ) : (
        <p style={{ color: "#4A5568", marginBottom: 8 }}>
          Ningún outcome coincide con el puntaje actual.
        </p>
      )}

      {/* Todas las reglas activas */}
      {all.length > 0 && (
        <>
          <div style={{ fontSize: 13, color: "#4A5568", margin: "10px 0 6px" }}>
            Todas las reglas activas:
          </div>
          <div>
            {all.map((o, i) => (
              <Chip
                key={`all-${o.id_outcome ?? i}`} // ✅ evita duplicados all-undefined
                text={o.name || `Outcome ${i + 1}`}
                color={severityToColor(o.severity_level)}
                subtle
                title={`${o.description || ""} · Rango: ${o.min_score}-${o.max_score}`}
              />
            ))}
          </div>
        </>
      )}
    </div>
  );
}
