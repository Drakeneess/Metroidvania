import { createContext, useContext, useEffect, useState } from "react";
import { endSessionBeacon, restoreSessionIdFromStorage, setToken, startSession } from "../../ingest/sessionIngest";

const AuthContext = createContext(null);
export const useAuth = () => useContext(AuthContext);

export function AuthProvider({ children }) {
  const [user, setUser] = useState(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const token = localStorage.getItem("token");
    const cached = localStorage.getItem("user");
    if (token) setToken(token);
    restoreSessionIdFromStorage();
    if (token && cached) {
      try { setUser(JSON.parse(cached)); } catch (e) {}
    }
    setLoading(false);

    const onUnload = () => endSessionBeacon();
    window.addEventListener("beforeunload", onUnload);
    return () => window.removeEventListener("beforeunload", onUnload);
  }, []);

  const login = async (data) => {
    // data: { token, user: {...} }
    localStorage.setItem("token", data.token);
    localStorage.setItem("user", JSON.stringify(data.user));
    setUser(data.user);
    setToken(data.token);

    await startSession(); // guarda sessionId
  };

  const logout = () => {
    endSessionBeacon();
    localStorage.removeItem("token");
    localStorage.removeItem("user");
    setUser(null);
    setToken(null);
  };

  return (
    <AuthContext.Provider value={{ user, loading, login, logout }}>
      {children}
    </AuthContext.Provider>
  );
}
