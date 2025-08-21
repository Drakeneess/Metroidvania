import { createContext, useContext, useEffect, useState } from "react";
import axios from "axios";

const AuthContext = createContext(null);
export const useAuth = () => useContext(AuthContext);

export function AuthProvider({ children }) {
  const [user, setUser] = useState(null);   // { id, full_name, email, role, state }
  const [loading, setLoading] = useState(true);

  // Cargar sesión desde token (opción A: confiar en lo que guardaste al login)
  useEffect(() => {
    const token = localStorage.getItem("token");
    const cached = localStorage.getItem("user");
    if (token && cached) {
      try {
        setUser(JSON.parse(cached));
      } catch {}
    }
    setLoading(false);
  }, []);

  // Opción B: validar con backend (descomenta si tienes /api/me)
  // useEffect(() => {
  //   const token = localStorage.getItem("token");
  //   if (!token) { setUser(null); setLoading(false); return; }
  //   axios.get("http://localhost:4000/api/me", {
  //     headers: { Authorization: `Bearer ${token}` }
  //   }).then(res => {
  //     setUser(res.data); // asegúrate que devuelva {id, full_name, email, role, state}
  //   }).catch(() => setUser(null)).finally(() => setLoading(false));
  // }, []);

  const login = (data) => {
    // data: { token, user: { id, full_name, email, role, state } }
    localStorage.setItem("token", data.token);
    localStorage.setItem("user", JSON.stringify(data.user));
    setUser(data.user);
  };

  const logout = () => {
    localStorage.removeItem("token");
    localStorage.removeItem("user");
    setUser(null);
  };

  return (
    <AuthContext.Provider value={{ user, loading, login, logout }}>
      {children}
    </AuthContext.Provider>
  );
}
