import { Spinner } from "@chakra-ui/react";
import { formatDate } from "./utils";

export default function StudentHeader({ student }) {
  if (!student)
    return (
      <div style={{ fontWeight: 700, fontSize: 18 }}>
        Detalle del estudiante
      </div>
    );

  // --- Verificación más estricta ---
  const alertColor = student.alert?.color;
  const alertType = student.alert?.type?.toLowerCase?.();
  const showAlert =
    alertColor && alertType && alertType !== "sin alerta"; // ✅ sólo si es una alerta real

  return (
    <div
      style={{
        display: "flex",
        alignItems: "center",
        justifyContent: "space-between",
        width: "100%",
      }}
    >
      {/* Info del estudiante */}
      <div style={{ display: "grid", gap: 6 }}>
        <div style={{ fontWeight: 700, fontSize: 18 }}>
          {student.full_name}
        </div>
        <div style={{ color: "#4A5568", fontSize: 14 }}>
          CI {student.ci} · Edad: {student.age_range} · Registro:{" "}
          {formatDate(student.register_date)}
        </div>
      </div>

      {/* Spinner de nivel de alerta (solo si aplica) */}
      {showAlert && (
        <Spinner
          size="lg"
          thickness="4px"
          speed="1s"
          color={alertColor}
          emptyColor="gray.200"
          title={student.alert?.type || "Alerta"}
          style={{ marginLeft: 16 }}
        />
      )}
    </div>
  );
}
