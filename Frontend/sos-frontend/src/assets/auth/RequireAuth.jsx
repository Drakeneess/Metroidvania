import { Navigate, useLocation } from "react-router-dom";
import { useAuth } from "./AuthContext";

export default function RequireAuth({ children, roles }) {
  const { user, loading } = useAuth();
  const loc = useLocation();

  if (loading) return null; // o un spinner
  if (!user) return <Navigate to="/" state={{ from: loc }} replace />;

  // roles es un array de strings, ej: ["admin", "gabinete"]
  if (roles && roles.length > 0 && !roles.includes(user.role)) {
    return <Navigate to="/forbidden" replace />;
  }
  return children;
}
