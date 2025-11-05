import { Box, ChakraProvider } from "@chakra-ui/react";
import { BrowserRouter as Router, Routes, Route } from "react-router-dom";
import { AuthProvider } from "./assets/auth/AuthContext";
import RequireAuth from "./assets/auth/RequireAuth";

import AppNav from "./assets/components/AppNav";
import LoginForm from "./assets/components/LoginForm";
import Dashboard from "./assets/components/Dashboard";
import AdminDashboard from "./assets/pages/AdminDashboard";
import PsychologistDashboard from "./assets/pages/PsychologistDashboard";
import TeacherDashboard from "./assets/pages/TeacherDashboard";
import Forbidden from "./assets/components/Forbidden";

// 🆕 imports nuevos
import GradientBackground from "./assets/components/GradientBackground";
import theme from "./theme";

export default function App() {
  return (
    <ChakraProvider theme={theme}>
      <AuthProvider>
        <Router>
          {/* 🌌 Fondo animado global */}
          <GradientBackground />

          <Box
            minH="100vh"
            bg="transparent"
            pos="relative"
            zIndex={1}
          >
            <AppNav />
            <Routes>
              <Route path="/" element={<LoginForm />} />

              {/* Cualquiera logueado */}
              <Route
                path="/dashboard"
                element={
                  <RequireAuth>
                    <Dashboard />
                  </RequireAuth>
                }
              />

              {/* Exclusivo ADMIN */}
              <Route
                path="/dashboard/admin"
                element={
                  <RequireAuth roles={["admin"]}>
                    <AdminDashboard />
                  </RequireAuth>
                }
              />

              {/* Exclusivo PSYCHOLOGIST */}
              <Route
                path="/dashboard/psychologist"
                element={
                  <RequireAuth roles={["psychologist", "admin"]}>
                    <PsychologistDashboard />
                  </RequireAuth>
                }
              />

              {/* Exclusivo TEACHER */}
              <Route
                path="/dashboard/teacher"
                element={
                  <RequireAuth roles={["teacher", "admin"]}>
                    <TeacherDashboard />
                  </RequireAuth>
                }
              />

              <Route path="/forbidden" element={<Forbidden />} />
            </Routes>
          </Box>
        </Router>
      </AuthProvider>
    </ChakraProvider>
  );
}
