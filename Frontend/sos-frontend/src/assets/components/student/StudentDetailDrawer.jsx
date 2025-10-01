import { useEffect } from "react";
import StudentSummary from "./StudentSummary";
import DistributionBars from "../bdi/BdiDistributionBars";
import OutcomeChips from "../bdi/StudentOutcomeChips";
import ResponsesTable from "../bdi/StudentResponsesTable";

export default function StudentDetailDrawer({ isOpen, onClose, loading, payload }) {
  const student = payload?.student;
  const stats = payload?.stats || {};
  const outcomes = payload?.outcomes || {};
  const items = payload?.items || {};

  // Evita scroll de fondo cuando el drawer está abierto
  useEffect(() => {
    if (isOpen) document.body.style.overflow = "hidden";
    else document.body.style.overflow = "";
    return () => (document.body.style.overflow = "");
  }, [isOpen]);

  return (
    <>
      {/* Overlay */}
      {isOpen && (
        <div
          onClick={onClose}
          style={{
            position: "fixed",
            inset: 0,
            background: "rgba(0,0,0,0.4)",
            zIndex: 1000
          }}
        />
      )}

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
            fontSize: 18
          }}
        >
          ×
        </button>

        {/* Header */}
        <header style={{ padding: 20, borderBottom: "1px solid #EDF2F7" }}>
          {student ? (
            <>
              <h2 style={{ fontWeight: 700, fontSize: 18 }}>{student.full_name}</h2>
              <div style={{ color: "#4A5568", fontSize: 14 }}>
                CI: {student.ci} · Edad: {student.age_range} · Registro:{" "}
                {new Date(student.register_date).toLocaleDateString()}
              </div>
            </>
          ) : (
            <h2>Detalle del estudiante</h2>
          )}
        </header>

        {/* Body */}
        <main style={{ padding: 20, overflowY: "auto", flex: 1 }}>
          {loading ? (
            <p>Cargando…</p>
          ) : !payload ? (
            <p style={{ color: "#718096" }}>Seleccione un estudiante para ver detalles.</p>
          ) : (
            <div style={{ display: "grid", gap: 24 }}>
              <StudentSummary stats={stats} />
              <DistributionBars distribution={stats.distribution} total={stats.answeredCount} />
              <OutcomeChips matched={outcomes.matched} all={outcomes.all} />
              <ResponsesTable answered={items.answered} unanswered={items.unanswered} />
            </div>
          )}
        </main>
      </aside>
    </>
  );
}
