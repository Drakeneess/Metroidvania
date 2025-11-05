export function Centered({ children }) {
  return (
    <div style={{ padding: "48px 0", display: "grid", placeItems: "center" }}>{children}</div>
  );
}
export function Grid({ cols = 3, children }) {
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
export function Card({ children }) {
  return (
    <div style={{ border: "1px solid #E2E8F0", borderRadius: 12, padding: 16 }}>{children}</div>
  );
}
export function Label({ children }) {
  return <div style={{ fontSize: 13, color: "#4A5568", marginBottom: 6 }}>{children}</div>;
}
export function Big({ children }) {
  return <div style={{ fontSize: 22, fontWeight: 700 }}>{children}</div>;
}
export function Medium({ children }) {
  return <div style={{ fontWeight: 600 }}>{children}</div>;
}
export function Small({ children }) {
  return <div style={{ fontSize: 13, color: "#4A5568", marginTop: 6 }}>{children}</div>;
}
export function SectionTitle({ children }) {
  return <div style={{ fontWeight: 600, marginBottom: 8 }}>{children}</div>;
}
export function Divider(props) {
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
export function Spinner() {
  const size = 36;
  const border = 4;
  const color = "#3182CE";
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
export function Progress({ value = 0 }) {
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
