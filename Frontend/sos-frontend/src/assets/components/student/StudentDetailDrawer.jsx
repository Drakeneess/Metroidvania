// src/components/student-detail/StudentDetailDrawer.jsx
import { useEffect, useMemo, useState } from "react";
import { Select, Tooltip } from "@chakra-ui/react";
import StudentSummary from "./StudentSummary";
import DistributionBars from "../bdi/BdiDistributionBars";
import OutcomeChips from "../bdi/StudentOutcomeChips";
import ResponsesTable from "../bdi/StudentResponsesTable";
import GameSessionsSection from "../GameSessions/GameSessionsSection";
import ExportSection from "../student-detail/ExportSection";
import { fetchGameSessions } from "../../services/gameSessionService";
import {
  getAlertForStudent,
  getAlertLevels,
  createAlert,
  updateAlert,
} from "../../services/alertService";

export default function StudentDetailDrawer({
  isOpen,
  onClose,
  loading,
  payload,
  authUser,
}) {
  const [activeTab, setActiveTab] = useState("responses");
  const [sessions, setSessions] = useState([]);
  const [sessionsLoading, setSessionsLoading] = useState(false);
  const [sessionsError, setSessionsError] = useState(null);

  // 🔹 Estado para alertas
  const [alertInfo, setAlertInfo] = useState(null);
  const [alertLevels, setAlertLevels] = useState([]);
  const [selectedAlert, setSelectedAlert] = useState("");

  const student = payload?.student;
  const stats = payload?.stats || {};
  const outcomes = payload?.outcomes || {};
  const items = payload?.items || {};
  const answered = items.answered || [];
  const unanswered = items.unanswered || [];

  // Bloquear scroll del body al abrir el drawer
  useEffect(() => {
    const prev = document.body.style.overflow;
    console.log(prev);
    document.body.style.overflow = isOpen ? "hidden" : prev || "";
    return () => {
      document.body.style.overflow = prev || "";
    };
  }, [isOpen]);

  // Reset de pestaña y sesiones al cambiar estudiante
  useEffect(() => {
    setActiveTab("responses");
    setSessions([]);
    setSessionsError(null);
  }, [payload]);

  // 🧩 Cargar metadatos de alertas (niveles)
  useEffect(() => {
    getAlertLevels()
      .then((levels) => setAlertLevels(levels || []))
      .catch((err) => console.error("❌ Error al cargar meta de alertas:", err));
  }, []);

  // 🔥 Cargar alerta actual del estudiante
  useEffect(() => {
    if (!student?.id_student) return;

    console.log("🟢 Solicitando alerta para:", student.id_student);

    getAlertForStudent(student.id_student)
      .then((alert) => {
        console.log("🟣 Respuesta del backend (alert):", alert);
        setAlertInfo(alert);
        setSelectedAlert(alert?.id_alert || "");
      })
      .catch((err) => {
        console.error("🔴 Error al cargar alerta:", err);
        setAlertInfo(null);
        setSelectedAlert("");
      });
  }, [student?.id_student]);

  // 🎛️ Cambiar o asignar alerta
  const handleAlertChange = async (e) => {
    const newId = e.target.value;
    setSelectedAlert(newId);
    if (!newId) return;

    try {
      if (!alertInfo) {
        // Crear nueva
        const created = await createAlert({
          id_student: student.id_student,
          id_alert: newId,
        });
        setAlertInfo(created);
      } else {
        // Actualizar existente
        const updated = await updateAlert({
          id_alert_student: alertInfo.id_alert_student,
          id_alert: newId,
        });
        setAlertInfo(updated);
      }
    } catch (err) {
      console.error("Error al actualizar alerta:", err);
    }
  };

  // Carga on-demand de Game Sessions
  useEffect(() => {
    if (activeTab === "sessions" && student) {
      setSessionsLoading(true);
      setSessionsError(null);

      fetchGameSessions(student.id_student)
        .then((sess) => {
          if (!sess || !sess.length) {
            setSessions([]);
          } else {
            setSessions(sess);
          }
        })
        .catch((err) => {
          console.error("Error al cargar sesiones:", err);

          if (err?.status === 404) {
            // <-- Bandera especial: no existen sesiones para este estudiante
            setSessions([]);
            setSessionsError("NO_SESSIONS"); 
          } else {
            setSessionsError(err?.message || "Error desconocido");
          }
        })
        .finally(() => setSessionsLoading(false));
    }
  }, [activeTab, student]);


  // 🎨 Estilos
    const styles = useMemo(
    () => ({
      overlay: {
        position: "fixed",
        inset: 0,
        background: "rgba(17, 17, 27, 0.45)",
        backdropFilter: "blur(2px)",
        opacity: isOpen ? 1 : 0,
        pointerEvents: isOpen ? "auto" : "none",
        transition: "opacity 220ms ease",
        zIndex: 1000,
      },
      drawer: {
        position: "fixed",
        top: 0,
        right: 0,
        width: "720px",
        maxWidth: "100vw",
        height: "100%",
        background:
          "linear-gradient(160deg, rgba(255,255,255,0.88) 0%, rgba(245,245,250,0.92) 100%)",
        boxShadow: "0 8px 32px rgba(0,0,0,0.25)",
        transform: isOpen ? "translateX(0%)" : "translateX(100%)",
        transition: "transform 280ms cubic-bezier(0.25,0.8,0.25,1)",
        zIndex: 1001,
        display: "flex",
        flexDirection: "column",
        borderLeft: "1px solid rgba(255,255,255,0.6)",
        backdropFilter: "blur(10px) saturate(160%)",
      },
      closeBtn: {
        position: "absolute",
        top: 14,
        right: 14,
        width: 36,
        height: 36,
        borderRadius: 12,
        border: "none",
        background:
          "linear-gradient(135deg, rgba(240,240,250,1), rgba(225,225,240,0.9))",
        cursor: "pointer",
        fontSize: 20,
        lineHeight: "34px",
        color: "#2D3748",
        boxShadow: "0 2px 6px rgba(0,0,0,0.15)",
        transition: "all 0.2s ease",
      },
      header: {
        position: "sticky",
        top: 0,
        zIndex: 2,
        padding: "18px 22px",
        borderBottom: "1px solid rgba(0,0,0,0.05)",
        background: "rgba(255,255,255,0.85)",
        backdropFilter: "blur(8px) saturate(180%)",
        boxShadow: "0 2px 10px rgba(0,0,0,0.04)",
      },
      tabsBar: {
        display: "flex",
        gap: 8,
        padding: "12px 20px",
        borderBottom: "1px solid rgba(0,0,0,0.06)",
        background: "rgba(255,255,255,0.75)",
        backdropFilter: "blur(6px) saturate(160%)",
        position: "sticky",
        top: 70,
        zIndex: 1,
      },
      tabBtn: (active) => ({
        padding: "8px 16px",
        borderRadius: 10,
        background: active
          ? "linear-gradient(135deg, #6B46C1 0%, #805AD5 100%)"
          : "rgba(245,245,255,0.8)",
        color: active ? "#fff" : "#2D3748",
        border: "1px solid " + (active ? "transparent" : "rgba(0,0,0,0.05)"),
        cursor: "pointer",
        fontSize: 14,
        fontWeight: active ? 700 : 600,
        transition:
          "box-shadow 160ms ease, transform 120ms ease, background 160ms ease",
        boxShadow: active
          ? "0 4px 12px rgba(107,70,193,0.3)"
          : "0 1px 3px rgba(0,0,0,0.06)",
      }),
      tabBtnHover: {
        boxShadow: "0 6px 16px rgba(0,0,0,0.12)",
        transform: "translateY(-1px)",
      },
      main: {
        padding: 24,
        overflowY: "auto",
        flex: 1,
        scrollBehavior: "smooth",
        background:
          "linear-gradient(180deg, rgba(255,255,255,0.95), rgba(250,250,255,0.9))",
      },
      sectionTitle: {
        fontWeight: 700,
        fontSize: 15,
        margin: "18px 0 10px",
        color: "#1A202C",
      },
      muted: {
        color: "#718096",
        fontSize: 14,
      },
      error: {
        color: "#E53E3E",
        fontWeight: 600,
      },
    }),
    [isOpen]
  );

  const [hoverTab, setHoverTab] = useState("");

  // ======================
  // 🔥 HEADER con dropdown de alerta
  // ======================
  return (
    <>
      <div onClick={onClose} style={styles.overlay} />

      <style>{`
        #sosDrawerScroll::-webkit-scrollbar { width: 10px; }
        #sosDrawerScroll::-webkit-scrollbar-thumb {
          background-color: rgba(0,0,0,0.15);
          border-radius: 10px;
          border: 3px solid transparent;
          background-clip: content-box;
        }
        #sosDrawerScroll { scrollbar-color: rgba(0,0,0,0.25) transparent; scrollbar-width: thin; }
      `}</style>

      <aside role="dialog" aria-modal="true" style={styles.drawer}>
        <button onClick={onClose} aria-label="Cerrar" style={styles.closeBtn}>
          ×
        </button>

        {/* Header con dropdown */}
        <header style={styles.header}>
          {student ? (
            <div
              style={{
                display: "flex",
                alignItems: "center",
                justifyContent: "space-between",
              }}
            >
              <div>
                <h2 style={{ fontWeight: 800, fontSize: 18, color: "#1A202C" }}>
                  {student.full_name}
                </h2>
                <div style={{ color: "#4A5568", fontSize: 14, marginTop: 2 }}>
                  CI: {student.ci} · Edad: {student.age_range} · Registro:{" "}
                  {student.register_date
                    ? new Date(student.register_date).toLocaleDateString()
                    : "—"}
                </div>
              </div>

              <Tooltip label="Nivel de alerta" hasArrow placement="bottom">
                <Select
                  value={selectedAlert}
                  onChange={handleAlertChange}
                  placeholder="Sin alerta"
                  width="180px"
                  size="sm"
                  ml={4}
                  fontWeight="600"
                  border="1px solid rgba(0,0,0,0.1)"
                  borderRadius="10px"
                  bg={
                    alertLevels.find((lvl) => String(lvl.id) === String(selectedAlert))
                      ? `${alertLevels.find((lvl) => String(lvl.id) === String(selectedAlert)).color}30`
                      : "rgba(255,255,255,0.8)"
                  } // color + transparencia (ej: #E53E3E30)
                  color={
                    alertLevels.find((lvl) => String(lvl.id) === String(selectedAlert))
                      ? "#1A202C"
                      : "#2D3748"
                  }
                  _hover={{
                    boxShadow: "0 2px 8px rgba(0,0,0,0.1)",
                    bg: "rgba(255,255,255,0.95)",
                  }}
                  _focus={{
                    borderColor: "#6B46C1",
                    boxShadow: "0 0 0 2px rgba(107,70,193,0.25)",
                  }}
                >
                  {alertLevels.map((level) => (
                    <option
                      key={level.id}
                      value={level.id}
                      style={{
                        background: "white",
                        color: "#1A202C",
                        fontWeight: 500,
                      }}
                    >
                      {level.type}
                    </option>
                  ))}
                </Select>
              </Tooltip>

            </div>
          ) : (
            <h2 style={{ fontWeight: 800, fontSize: 18, color: "#1A202C" }}>
              Detalle del estudiante
            </h2>
          )}
        </header>

        {/* Tabs */}
        <div style={styles.tabsBar}>
          <TabButton
            active={activeTab === "responses"}
            onClick={() => setActiveTab("responses")}
            onHover={hoverTab === "responses"}
            setHover={(v) => setHoverTab(v ? "responses" : "")}
            styles={styles}
          >
            Respuestas
          </TabButton>
          <TabButton
            active={activeTab === "sessions"}
            onClick={() => setActiveTab("sessions")}
            onHover={hoverTab === "sessions"}
            setHover={(v) => setHoverTab(v ? "sessions" : "")}
            styles={styles}
          >
            Game Sessions
          </TabButton>
          <TabButton
            active={activeTab === "export"}
            onClick={() => setActiveTab("export")}
            onHover={hoverTab === "export"}
            setHover={(v) => setHoverTab(v ? "export" : "")}
            styles={styles}
          >
            Exportar
          </TabButton>
        </div>

        {/* Contenido */}
        <main id="sosDrawerScroll" style={styles.main}>
          {loading ? (
            <p>Cargando…</p>
          ) : !payload ? (
            <p style={styles.muted}>Seleccione un estudiante para ver detalles.</p>
          ) : (
            <>
              <StudentSummary stats={stats} />

              {activeTab === "responses" && (
                <>
                  <h3 style={styles.sectionTitle}>Distribución de respuestas</h3>
                  <DistributionBars
                    distribution={stats.distribution}
                    total={stats.answeredCount}
                  />
                  <h3 style={styles.sectionTitle}>Interpretación / Outcomes</h3>
                  <OutcomeChips
                    matched={outcomes.matched || []}
                    all={outcomes.all || []}
                  />
                  <h3 style={styles.sectionTitle}>Respuestas</h3>
                  <ResponsesTable answered={answered} unanswered={unanswered} />
                </>
              )}

              {activeTab === "sessions" && (
                <>
                  {sessionsLoading ? (
                    <p>Cargando sesiones…</p>
                  ) : (
                    <GameSessionsSection sessions={sessions} error={sessionsError} />
                  )}
                </>
              )}


              {activeTab === "export" && (
                <ExportSection
                  payload={payload}
                  sessions={sessions}
                  authUser={authUser}
                />
              )}
            </>
          )}
        </main>
      </aside>
    </>
  );
}

function TabButton({ active, onClick, children, onHover, setHover, styles }) {
  const base = styles.tabBtn(active);
  const hover = onHover ? styles.tabBtnHover : null;

  return (
    <button
      onClick={onClick}
      onMouseEnter={() => setHover(true)}
      onMouseLeave={() => setHover(false)}
      style={hover ? { ...base, ...hover } : base}
    >
      {children}
    </button>
  );
}
