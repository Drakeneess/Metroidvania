import { api } from "../lib/axiosInstance";

const API_BASE = import.meta.env.VITE_API_URL || "http://localhost:4000";

let SESSION_ID: string | null = null;
let TOKEN: string | null = null;

export function getSessionId(): string | null { return SESSION_ID; }
export function setToken(t: string | null) { TOKEN = t ?? null; }

type StartResp = { ok?: boolean; sessionId?: string };

export async function startSession(): Promise<string | null> {
  if (!TOKEN) return null;
  const { data } = await api.post<StartResp>("/ingest/admin/session/start");
  if (data?.ok && typeof data.sessionId === "string") {
    const sid = data.sessionId;
    localStorage.setItem("admin_session_id", sid);
    SESSION_ID = sid;
    return sid;
  }
  return null;
}

export function restoreSessionIdFromStorage(): void {
  const sid = localStorage.getItem("admin_session_id");
  SESSION_ID = sid || null;
}

export async function endSessionBeacon(): Promise<void> {
  if (!SESSION_ID) return;
  const token = localStorage.getItem("token") || "";
  try {
    await fetch(`${API_BASE}/ingest/admin/session/end`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
        "Authorization": `Bearer ${token}`, // 👈 ahora sí va el JWT
      },
      body: JSON.stringify({ sessionId: SESSION_ID }),
      keepalive: true, // 👈 soporta requests en unload
    });
  } catch (err) {
    console.warn("endSessionKeepalive error:", err);
  }
  localStorage.removeItem("admin_session_id");
  SESSION_ID = null;
}


export async function heartbeat(): Promise<void> {
  if (!TOKEN || !SESSION_ID) return;
  await fetch(`${API_BASE}/ingest/admin/session/heartbeat`, {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
      Authorization: `Bearer ${TOKEN}`,
    },
    body: JSON.stringify({ sessionId: SESSION_ID }),
  });
}
