import { Box } from "@chakra-ui/react";
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
import NotFound from "./assets/components/NotFound";

import GradientBackground from "./assets/components/GradientBackground";

export default function App() {
  return (
    <AuthProvider>
      <Router>
        <GradientBackground />

        <Box
          minH="100vh"
          bg="transparent"
          pos="relative"
          zIndex={1}
        >
          <AppNav />

          <Box as="main" px={{ base: 4, md: 8 }} py={{ base: 4, md: 6 }}>
            <Routes>
              <Route path="/" element={<LoginForm />} />

              <Route
                path="/dashboard"
                element={
                  <RequireAuth>
                    <Dashboard />
                  </RequireAuth>
                }
              />

              <Route
                path="/dashboard/admin"
                element={
                  <RequireAuth roles={["admin"]}>
                    <AdminDashboard />
                  </RequireAuth>
                }
              />

              <Route
                path="/dashboard/psychologist"
                element={
                  <RequireAuth roles={["psychologist", "admin"]}>
                    <PsychologistDashboard />
                  </RequireAuth>
                }
              />

              <Route
                path="/dashboard/teacher"
                element={
                  <RequireAuth roles={["teacher", "admin"]}>
                    <TeacherDashboard />
                  </RequireAuth>
                }
              />

              <Route path="/forbidden" element={<Forbidden />} />
              <Route path="*" element={<NotFound />} />
            </Routes>
          </Box>
        </Box>
      </Router>
    </AuthProvider>
  );
}