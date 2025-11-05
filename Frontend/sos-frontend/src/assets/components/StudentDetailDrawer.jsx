// src/components/StudentDetailDrawer.jsx
export default function StudentDetailDrawer({ isOpen, onClose, loading, payload }) {
  const student = payload?.student;
  const stats = payload?.stats || {};
  const outcomes = payload?.outcomes || {};
  const items = payload?.items || {};

  return (
    <>
      {/* Overlay */}
      <div
        onClick={onClose}
        style={{
          position: "fixed",
          inset: 0,
          background: "rgba(0,0,0,0.4)",
          opacity: isOpen ? 1 : 0,
          pointerEvents: isOpen ? "auto" : "none",
          transition: "opacity 200ms ease",
          zIndex: 1000
        }}
      />

      {/* Drawer panel */}
      <aside
        role="dialog"
        aria-modal="true"
        style={{
          position: "fixed",
          top: 0,
          right: 0,
          width: "720px",
          maxWidth: "100vw",
          height: "100%",
          background: "#fff",
          boxShadow: "0 10px 30px rgba(0,0,0,0.2)",
          transform: isOpen ? "translateX(0%)" : "translateX(100%)",
          transition: "transform 260ms ease",
          zIndex: 1001,
          display: "flex",
          flexDirection: "column"
        }}
      >
        {/* Close */}
        <button
          onClick={onClose}
          aria-label="Cerrar"
          style={{
            position: "absolute",
            top: 12,
            right: 12,
            width: 34,
            height: 34,
            borderRadius: 8,
            border: "1px solid #E2E8F0",
            background: "#fff",
            cursor: "pointer",
            fontSize: 18,
            lineHeight: "32px"
          }}
        >
          ×
        </button>

        {/* Header */}
        <header style={{ padding: "18px 20px", borderBottom: "1px solid #EDF2F7" }}>
          {student ? (
            <div style={{ display: "grid", gap: 6 }}>
              <div style={{ fontWeight: 700, fontSize: 18 }}>{student.full_name}</div>
              <div style={{ color: "#4A5568", fontSize: 14 }}>
                C: {student.ci} · Edad: {student.age_range} · Registro:{" "}
                {formatDate(student.register_date)}
              </div>
            </div>
          ) : (
            <div style={{ fontWeight: 700, fontSize: 18 }}>Detalle del estudiante</div>
          )}
        </header>

        {/* Body */}
        <main style={{ padding: 20, overflowY: "auto", flex: 1 }}>
          {loading ? (
            <Centered>
              <Spinner />
            </Centered>
          ) : !payload ? (
            <p style={{ color: "#718096" }}>Seleccione un estudiante para ver detalles.</p>
          ) : (
            <div style={{ display: "grid", gap: 24 }}>
              {/* Resumen */}
              <Grid cols={3}>
                <Card>
                  <Label>Puntaje total</Label>
                  <Big>{safeNum(stats.totalScore)}</Big>
                </Card>
                <Card>
                  <Label>Avance</Label>
                  <Progress value={safeNum(stats.completion)} />
                  <Small>
                    {safeNum(stats.answeredCount)}/{safeNum(stats.totalItems)} (
                    {safeNum(stats.completion)}%)
                  </Small>
                </Card>
                <Card>
                  <Label>Última respuesta</Label>
                  <Medium>
                    {stats.lastAnswerAt ? new Date(stats.lastAnswerAt).toLocaleString() : "—"}
                  </Medium>
                </Card>
              </Grid>

              {/* Distribución */}
              <SectionTitle>Distribución de respuestas</SectionTitle>
              <DistributionBars
                distribution={stats.distribution || {}}
                total={safeNum(stats.answeredCount)}
              />

              <Divider />

              {/* Outcomes */}
              <SectionTitle>Interpretación / Outcomes</SectionTitle>
              <OutcomeChips matched={outcomes.matched || []} all={outcomes.all || []} />

              <Divider />

              {/* Respuestas */}
              <SectionTitle>Respuestas</SectionTitle>
              <ResponsesTable
                answered={items.answered || []}
                unanswered={items.unanswered || []}
              />
            </div>
          )}
        </main>
      </aside>
    </>
  );
}

/* ---------- Utilidades UI sin imports ---------- */

function formatDate(d) {
  try {
    return new Date(d).toLocaleDateString();
  } catch {
    return "—";
  }
}
function safeNum(n, fallback = 0) {
  return typeof n === "number" && !Number.isNaN(n) ? n : fallback;
}

/* Layout helpers */
function Centered({ children }) {
  return (
    <div style={{ padding: "48px 0", display: "grid", placeItems: "center" }}>{children}</div>
  );
}
function Grid({ cols = 3, children }) {
  return (
    <div
      style={{
        display: "grid",
        gap: 16,
        gridTemplateColumns: `repeat(${cols}, minmax(0, 1fr))`
      }}
    >
      {children}
    </div>
  );
}
function Card({ children }) {
  return (
    <div style={{ border: "1px solid #E2E8F0", borderRadius: 12, padding: 16 }}>{children}</div>
  );
}
function Label({ children }) {
  return <div style={{ fontSize: 13, color: "#4A5568", marginBottom: 6 }}>{children}</div>;
}
function Big({ children }) {
  return <div style={{ fontSize: 22, fontWeight: 700 }}>{children}</div>;
}
function Medium({ children }) {
  return <div style={{ fontWeight: 600 }}>{children}</div>;
}
function Small({ children }) {
  return <div style={{ fontSize: 13, color: "#4A5568", marginTop: 6 }}>{children}</div>;
}
function SectionTitle({ children }) {
  return <div style={{ fontWeight: 600, marginBottom: 8 }}>{children}</div>;
}
function Divider(props) {
  return (
    <hr
      {...props}
      style={{
        border: 0,
        borderTop: "1px solid #E2E8F0",
        margin: "8px 0"
      }}
    />
  );
}

/* Spinner minimalista */
function Spinner() {
  const size = 36;
  const border = 4;
  const color = "#3182CE"; // azul
  return (
    <div
      aria-label="Cargando"
      style={{
        width: size,
        height: size,
        border: `${border}px solid #E2E8F0`,
        borderTopColor: color,
        borderRadius: "50%",
        animation: "spin 0.8s linear infinite"
      }}
    >
      <style>{`@keyframes spin { to { transform: rotate(360deg); } }`}</style>
    </div>
  );
}

/* Progress bar simple */
function Progress({ value = 0 }) {
  const v = Math.max(0, Math.min(100, value));
  return (
    <div
      style={{
        marginTop: 8,
        height: 8,
        background: "#EDF2F7",
        borderRadius: 6,
        overflow: "hidden"
      }}
    >
      <div
        style={{
          width: `${v}%`,
          height: "100%",
          background: "#3182CE",
          transition: "width 260ms ease"
        }}
      />
    </div>
  );
}

/* Distribución de respuestas (0..3) */
function DistributionBars({ distribution = {}, total = 0 }) {
  const entries = Object.entries(distribution).sort(
    (a, b) => Number(a[0]) - Number(b[0])
  );
  if (!entries.length) return <p style={{ color: "#718096" }}>Sin datos.</p>;

  return (
    <div style={{ display: "grid", gap: 10 }}>
      {entries.map(([score, count]) => {
        const pct = total > 0 ? Math.round((count / total) * 100) : 0;
        return (
          <div key={score}>
            <div
              style={{
                display: "flex",
                justifyContent: "space-between",
                fontSize: 13,
                marginBottom: 6
              }}
            >
              <span>Score {score}</span>
              <span style={{ color: "#4A5568" }}>
                {count} ({pct}%)
              </span>
            </div>
            <Progress value={pct} />
          </div>
        );
      })}
    </div>
  );
}

/* Chips de outcomes */
function OutcomeChips({ matched = [], all = [] }) {
  const colorFor = (sev) => {
    switch ((sev || "").toLowerCase()) {
      case "mild":
        return "#38A169"; // green
      case "moderate":
        return "#D69E2E"; // yellow
      case "severe":
        return "#E53E3E"; // red
      default:
        return "#3182CE"; // blue
    }
  };

  const Chip = ({ text, color = "#3182CE", subtle = false, title }) => (
    <span
      title={title || ""}
      style={{
        display: "inline-block",
        padding: "4px 8px",
        borderRadius: 999,
        border: subtle ? `1px solid ${lighten(color, 0.5)}` : "none",
        background: subtle ? "#fff" : color,
        color: subtle ? "#2D3748" : "#fff",
        fontSize: 12,
        marginRight: 6,
        marginBottom: 6
      }}
    >
      {text}
    </span>
  );

  return (
    <div>
      {matched.length ? (
        <>
          <div style={{ fontSize: 13, color: "#4A5568", marginBottom: 6 }}>
            Que aplican:
          </div>
          <div>
            {matched.map((o) => (
              <Chip
                key={o.id_outcome}
                text={`${o.name}${o.narrative_flag ? ` · ${o.narrative_flag}` : ""}`}
                color={colorFor(o.severity_level)}
                title={o.description || ""}
              />
            ))}
          </div>
        </>
      ) : (
        <p style={{ color: "#718096", marginBottom: 8 }}>
          Ningún outcome coincide con el puntaje actual.
        </p>
      )}

      {all.length ? (
        <>
          <div style={{ fontSize: 13, color: "#4A5568", margin: "10px 0 6px" }}>
            Todas las reglas activas:
          </div>
          <div>
            {all.map((o) => (
              <Chip
                key={`all-${o.id_outcome}`}
                text={o.name}
                color={colorFor(o.severity_level)}
                subtle
                title={`${o.description || ""} · Rango: ${o.min_score}-${o.max_score}`}
              />
            ))}
          </div>
        </>
      ) : null}
    </div>
  );
}

/* Tabla de respuestas */
function ResponsesTable({ answered = [], unanswered = [] }) {
  const Row = ({ cols = [], header = false, keyProp }) => (
    <div
      key={keyProp}
      style={{
        display: "grid",
        gridTemplateColumns: "60px 2fr 2fr 80px",
        padding: "10px 12px",
        borderBottom: "1px solid #E2E8F0",
        background: header ? "#F7FAFC" : "transparent",
        fontWeight: header ? 600 : 400
      }}
    >
      {cols.map((c, i) => (
        <div
          key={i}
          style={{
            overflow: "hidden",
            textOverflow: "ellipsis",
            whiteSpace: "nowrap",
            textAlign: i === 3 ? "right" : "left",
            paddingRight: 8
          }}
          title={typeof c === "string" ? c : undefined}
        >
          {c}
        </div>
      ))}
    </div>
  );

  const Badge = ({ children }) => (
    <span
      style={{
        display: "inline-block",
        padding: "2px 8px",
        borderRadius: 8,
        border: "1px solid #CBD5E0",
        fontSize: 12
      }}
    >
      {children}
    </span>
  );

  return (
    <div style={{ border: "1px solid #E2E8F0", borderRadius: 12, overflow: "hidden" }}>
      <Row header cols={["#", "Ítem", "Respuesta", "Score"]} keyProp="head" />

      {answered.map((row) => (
        <Row
          keyProp={`ans-${row.id_item}`}
          cols={[
            String(row.item_number),
            row.title || "—",
            row.response
              ? row.response.response_symbol || row.response.response || "—"
              : "—",
            <Badge key="b">{typeof row.response?.score === "number" ? row.response.score : "0"}</Badge>
          ]}
        />
      ))}

      {unanswered.length > 0 && (
        <>
          <div
            style={{
              background: "#F7FAFC",
              padding: "8px 12px",
              fontWeight: 600,
              borderTop: "1px solid #E2E8F0"
            }}
          >
            Pendientes
          </div>
          {unanswered.map((row) => (
            <Row
              keyProp={`un-${row.id_item}`}
              cols={[
                String(row.item_number),
                row.title || "—",
                "Sin respuesta",
                <Badge key="b">—</Badge>
              ]}
            />
          ))}
        </>
      )}
    </div>
  );
}

/* Helpers visuales */
function lighten(hex, amt = 0.5) {
  // hex #RRGGBB
  const clamp = (n) => Math.max(0, Math.min(255, n));
  const h = (hex || "#3182CE").replace("#", "");
  const r = clamp(Math.round(parseInt(h.substring(0, 2), 16) + 255 * amt));
  const g = clamp(Math.round(parseInt(h.substring(2, 4), 16) + 255 * amt));
  const b = clamp(Math.round(parseInt(h.substring(4, 6), 16) + 255 * amt));
  return `rgb(${r}, ${g}, ${b})`;
}
