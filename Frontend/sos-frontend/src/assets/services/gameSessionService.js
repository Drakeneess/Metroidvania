const ANALYTICS_URL = import.meta.env.VITE_ANALYTICS_URL;

export async function fetchGameSessions(studentId) {
  const url = `${ANALYTICS_URL}/player_session_analysis/${studentId}`;

  const resp = await fetch(url);

  if (!resp.ok) {
    if (resp.status === 404) {
      // No hay sesiones registradas para el estudiante
      return [];
    }
    throw new Error(`Error al cargar sesiones (${resp.status})`);
  }

  const data = await resp.json();
  return data.sessions || [];
}
