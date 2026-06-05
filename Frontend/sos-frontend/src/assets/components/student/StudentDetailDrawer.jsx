// src/components/student-detail/StudentDetailDrawer.jsx

import { useEffect, useMemo, useState } from "react";
import {
  Box,
  Button,
  Flex,
  HStack,
  Select,
  Spinner,
  Text,
  Tooltip,
  useToast,
} from "@chakra-ui/react";

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

function formatDate(value) {
  if (!value) return "—";

  const date = new Date(value);

  if (Number.isNaN(date.getTime())) return "—";

  return date.toLocaleDateString("es-BO", {
    day: "2-digit",
    month: "2-digit",
    year: "numeric",
  });
}

function getLevelById(levels, id) {
  return levels.find((level) => String(level.id) === String(id));
}

export default function StudentDetailDrawer({
  isOpen,
  onClose,
  loading,
  payload,
  authUser,
}) {
  const toast = useToast();

  const [activeTab, setActiveTab] = useState("responses");
  const [hoverTab, setHoverTab] = useState("");

  const [sessions, setSessions] = useState([]);
  const [sessionsLoading, setSessionsLoading] = useState(false);
  const [sessionsError, setSessionsError] = useState(null);

  const [alertInfo, setAlertInfo] = useState(null);
  const [alertLevels, setAlertLevels] = useState([]);
  const [selectedAlert, setSelectedAlert] = useState("");
  const [alertUpdating, setAlertUpdating] = useState(false);

  const student = payload?.student;
  const stats = payload?.stats || {};
  const outcomes = payload?.outcomes || {};
  const items = payload?.items || {};

  const answered = items.answered || [];
  const unanswered = items.unanswered || [];

  const selectedLevel = useMemo(
    () => getLevelById(alertLevels, selectedAlert),
    [alertLevels, selectedAlert]
  );

  const alertSelectBg = selectedLevel?.color
    ? `${selectedLevel.color}26`
    : "rgba(255,255,255,0.82)";

  useEffect(() => {
    const previousOverflow = document.body.style.overflow;

    if (isOpen) {
      document.body.style.overflow = "hidden";
    }

    return () => {
      document.body.style.overflow = previousOverflow || "";
    };
  }, [isOpen]);

  useEffect(() => {
    setActiveTab("responses");
    setSessions([]);
    setSessionsError(null);
  }, [payload?.student?.id_student]);

  useEffect(() => {
    getAlertLevels()
      .then((levels) => setAlertLevels(levels || []))
      .catch((error) => {
        console.error("Error al cargar niveles de alerta:", error);
      });
  }, []);

  useEffect(() => {
    if (!student?.id_student || !isOpen) return;

    getAlertForStudent(student.id_student)
      .then((alert) => {
        setAlertInfo(alert);
        setSelectedAlert(alert?.id_alert || "");
      })
      .catch((error) => {
        console.error("Error al cargar alerta:", error);
        setAlertInfo(null);
        setSelectedAlert("");
      });
  }, [student?.id_student, isOpen]);

  useEffect(() => {
    if (activeTab !== "sessions" || !student?.id_student || !isOpen) return;

    setSessionsLoading(true);
    setSessionsError(null);

    fetchGameSessions(student.id_student)
      .then((sessionList) => {
        setSessions(Array.isArray(sessionList) ? sessionList : []);
      })
      .catch((error) => {
        console.error("Error al cargar sesiones:", error);

        if (error?.status === 404) {
          setSessions([]);
          setSessionsError("NO_SESSIONS");
          return;
        }

        setSessions([]);
        setSessionsError(error?.message || "Error desconocido");
      })
      .finally(() => setSessionsLoading(false));
  }, [activeTab, student?.id_student, isOpen]);

  const handleAlertChange = async (event) => {
    const newId = event.target.value;

    setSelectedAlert(newId);

    if (!newId || !student?.id_student) return;

    setAlertUpdating(true);

    try {
      if (!alertInfo) {
        const created = await createAlert({
          id_student: student.id_student,
          id_alert: newId,
        });

        setAlertInfo(created);
      } else {
        const updated = await updateAlert({
          id_alert_student: alertInfo.id_alert_student,
          id_alert: newId,
        });

        setAlertInfo(updated);
      }

      toast({
        title: "Alerta actualizada",
        status: "success",
        duration: 1800,
        isClosable: true,
      });
    } catch (error) {
      console.error("Error al actualizar alerta:", error);

      toast({
        title: "No se pudo actualizar la alerta",
        description: "Revisa la conexión o el endpoint de alertas.",
        status: "error",
        duration: 2400,
        isClosable: true,
      });
    } finally {
      setAlertUpdating(false);
    }
  };

  const styles = useMemo(
    () => ({
      overlay: {
        position: "fixed",
        inset: 0,
        background: "rgba(17, 17, 27, 0.45)",
        backdropFilter: "blur(3px)",
        opacity: isOpen ? 1 : 0,
        pointerEvents: isOpen ? "auto" : "none",
        transition: "opacity 220ms ease",
        zIndex: 1000,
      },

      drawer: {
        position: "fixed",
        top: 0,
        right: 0,
        width: "760px",
        maxWidth: "100vw",
        height: "100%",
        background:
          "linear-gradient(160deg, rgba(255,255,255,0.92) 0%, rgba(246,244,255,0.96) 48%, rgba(239,242,255,0.94) 100%)",
        boxShadow: "0 8px 42px rgba(15,12,41,0.32)",
        transform: isOpen ? "translateX(0%)" : "translateX(100%)",
        transition: "transform 280ms cubic-bezier(0.25,0.8,0.25,1)",
        zIndex: 1001,
        display: "flex",
        flexDirection: "column",
        borderLeft: "1px solid rgba(255,255,255,0.72)",
        backdropFilter: "blur(12px) saturate(160%)",
      },

      closeBtn: {
        position: "absolute",
        top: 14,
        right: 14,
        width: 36,
        height: 36,
        borderRadius: 12,
        border: "1px solid rgba(107,70,193,0.14)",
        background:
          "linear-gradient(135deg, rgba(255,255,255,1), rgba(238,235,252,0.95))",
        cursor: "pointer",
        fontSize: 20,
        lineHeight: "32px",
        color: "#2D3748",
        boxShadow: "0 2px 8px rgba(15,12,41,0.13)",
        transition: "all 0.2s ease",
      },

      header: {
        position: "sticky",
        top: 0,
        zIndex: 2,
        padding: "18px 22px",
        paddingRight: "62px",
        borderBottom: "1px solid rgba(107,70,193,0.12)",
        background: "rgba(255,255,255,0.86)",
        backdropFilter: "blur(10px) saturate(180%)",
        boxShadow: "0 2px 14px rgba(15,12,41,0.06)",
      },

      tabsBar: {
        display: "flex",
        gap: 8,
        padding: "12px 20px",
        borderBottom: "1px solid rgba(107,70,193,0.10)",
        background: "rgba(255,255,255,0.72)",
        backdropFilter: "blur(8px) saturate(160%)",
        position: "sticky",
        top: 76,
        zIndex: 1,
        overflowX: "auto",
      },

      tabBtn: (active) => ({
        padding: "8px 16px",
        borderRadius: 999,
        background: active
          ? "linear-gradient(135deg, #6B46C1 0%, #805AD5 100%)"
          : "rgba(248,247,255,0.92)",
        color: active ? "#fff" : "#2D3748",
        border: active
          ? "1px solid transparent"
          : "1px solid rgba(107,70,193,0.10)",
        cursor: "pointer",
        fontSize: 14,
        fontWeight: active ? 800 : 650,
        transition:
          "box-shadow 160ms ease, transform 120ms ease, background 160ms ease",
        boxShadow: active
          ? "0 6px 16px rgba(107,70,193,0.30)"
          : "0 1px 4px rgba(15,12,41,0.07)",
        whiteSpace: "nowrap",
      }),

      tabBtnHover: {
        boxShadow: "0 7px 18px rgba(15,12,41,0.16)",
        transform: "translateY(-1px)",
      },

      main: {
        padding: 24,
        overflowY: "auto",
        flex: 1,
        scrollBehavior: "smooth",
        background:
          "linear-gradient(180deg, rgba(255,255,255,0.96), rgba(250,249,255,0.94))",
      },

      sectionTitle: {
        fontWeight: 800,
        fontSize: 15,
        margin: "20px 0 10px",
        color: "#1A202C",
        letterSpacing: "-0.01em",
      },

      muted: {
        color: "#718096",
        fontSize: 14,
      },

      error: {
        color: "#E53E3E",
        fontWeight: 700,
      },
    }),
    [isOpen]
  );

  return (
    <>
      <Box onClick={onClose} style={styles.overlay} />

      <style>{`
        #sosDrawerScroll::-webkit-scrollbar { width: 10px; }
        #sosDrawerScroll::-webkit-scrollbar-thumb {
          background-color: rgba(107,70,193,0.22);
          border-radius: 10px;
          border: 3px solid transparent;
          background-clip: content-box;
        }
        #sosDrawerScroll::-webkit-scrollbar-thumb:hover {
          background-color: rgba(107,70,193,0.34);
        }
        #sosDrawerScroll {
          scrollbar-color: rgba(107,70,193,0.30) transparent;
          scrollbar-width: thin;
        }
      `}</style>

      <aside role="dialog" aria-modal="true" style={styles.drawer}>
        <button onClick={onClose} aria-label="Cerrar" style={styles.closeBtn}>
          ×
        </button>

        <header style={styles.header}>
          {student ? (
            <Flex
              align={{ base: "flex-start", md: "center" }}
              justify="space-between"
              direction={{ base: "column", md: "row" }}
              gap={3}
            >
              <Box>
                <Text
                  as="h2"
                  fontWeight="800"
                  fontSize="lg"
                  color="#1A202C"
                  lineHeight="1.2"
                >
                  {student.full_name || "Estudiante sin nombre"}
                </Text>

                <Text color="#4A5568" fontSize="sm" mt={1}>
                  CI: {student.ci || "—"} · Edad: {student.age_range || "—"} ·
                  Registro: {formatDate(student.register_date)}
                </Text>

                {authUser?.role && (
                  <Text color="#805AD5" fontSize="xs" fontWeight="700" mt={1}>
                    Vista actual: {authUser.role}
                  </Text>
                )}
              </Box>

              <Tooltip label="Nivel de alerta" hasArrow placement="bottom">
                <Box minW="180px">
                  <Select
                    value={selectedAlert}
                    onChange={handleAlertChange}
                    placeholder="Sin alerta"
                    width="180px"
                    size="sm"
                    isDisabled={alertUpdating}
                    fontWeight="700"
                    border="1px solid rgba(107,70,193,0.16)"
                    borderRadius="10px"
                    bg={alertSelectBg}
                    color="#1A202C"
                    _hover={{
                      boxShadow: "0 2px 8px rgba(15,12,41,0.12)",
                      bg: "rgba(255,255,255,0.96)",
                    }}
                    _focus={{
                      borderColor: "#6B46C1",
                      boxShadow: "0 0 0 2px rgba(107,70,193,0.25)",
                    }}
                    sx={{
                      option: {
                        background: "white",
                        color: "#1A202C",
                        fontWeight: 600,
                      },
                    }}
                  >
                    {alertLevels.map((level) => (
                      <option key={level.id} value={level.id}>
                        {level.type}
                      </option>
                    ))}
                  </Select>
                </Box>
              </Tooltip>
            </Flex>
          ) : (
            <Text as="h2" fontWeight="800" fontSize="lg" color="#1A202C">
              Detalle del estudiante
            </Text>
          )}
        </header>

        <div style={styles.tabsBar}>
          <TabButton
            active={activeTab === "responses"}
            onClick={() => setActiveTab("responses")}
            onHover={hoverTab === "responses"}
            setHover={(value) => setHoverTab(value ? "responses" : "")}
            styles={styles}
          >
            Respuestas
          </TabButton>

          <TabButton
            active={activeTab === "sessions"}
            onClick={() => setActiveTab("sessions")}
            onHover={hoverTab === "sessions"}
            setHover={(value) => setHoverTab(value ? "sessions" : "")}
            styles={styles}
          >
            Game Sessions
          </TabButton>

          <TabButton
            active={activeTab === "export"}
            onClick={() => setActiveTab("export")}
            onHover={hoverTab === "export"}
            setHover={(value) => setHoverTab(value ? "export" : "")}
            styles={styles}
          >
            Exportar
          </TabButton>
        </div>

        <main id="sosDrawerScroll" style={styles.main}>
          {loading ? (
            <Flex minH="320px" align="center" justify="center" direction="column">
              <Spinner size="lg" color="purple.500" />
              <Text mt={3} color="#718096" fontWeight="600">
                Cargando detalle…
              </Text>
            </Flex>
          ) : !payload ? (
            <Box
              p={5}
              border="1px dashed rgba(107,70,193,0.22)"
              borderRadius="16px"
              bg="rgba(255,255,255,0.72)"
            >
              <Text color="#718096">
                Seleccione un estudiante para ver detalles.
              </Text>
            </Box>
          ) : (
            <>
              <StudentSummary stats={stats} />

              {activeTab === "responses" && (
                <>
                  <h3 style={styles.sectionTitle}>
                    Distribución de respuestas
                  </h3>

                  <DistributionBars
                    distribution={stats.distribution || {}}
                    total={stats.answeredCount || 0}
                  />

                  <h3 style={styles.sectionTitle}>
                    Interpretación / Outcomes
                  </h3>

                  <OutcomeChips
                    matched={outcomes.matched || []}
                    all={outcomes.all || []}
                  />

                  <h3 style={styles.sectionTitle}>Respuestas</h3>

                  <ResponsesTable
                    answered={answered}
                    unanswered={unanswered}
                  />
                </>
              )}

              {activeTab === "sessions" && (
                <>
                  {sessionsLoading ? (
                    <Flex
                      minH="220px"
                      align="center"
                      justify="center"
                      direction="column"
                    >
                      <Spinner size="md" color="purple.500" />
                      <Text mt={3} color="#718096" fontWeight="600">
                        Cargando sesiones…
                      </Text>
                    </Flex>
                  ) : (
                    <GameSessionsSection
                      sessions={sessions}
                      error={sessionsError}
                    />
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

              <HStack mt={8} justify="flex-end">
                <Button
                  size="sm"
                  variant="outline"
                  borderColor="rgba(107,70,193,0.24)"
                  color="#6B46C1"
                  _hover={{
                    bg: "rgba(107,70,193,0.08)",
                  }}
                  onClick={onClose}
                >
                  Cerrar
                </Button>
              </HStack>
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
      type="button"
      onClick={onClick}
      onMouseEnter={() => setHover(true)}
      onMouseLeave={() => setHover(false)}
      style={hover ? { ...base, ...hover } : base}
    >
      {children}
    </button>
  );
}