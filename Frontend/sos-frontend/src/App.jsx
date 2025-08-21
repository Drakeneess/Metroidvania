import { Box } from "@chakra-ui/react";
import { BrowserRouter as Router, Routes, Route } from "react-router-dom";
import { AuthProvider } from "./assets/auth/AuthContext";
import RequireAuth from "./assets/auth/RequireAuth";

import AppNav from "./assets/components/AppNav";
import LoginForm from "./assets/components/LoginForm";
import Dashboard from "./assets/components/Dashboard";
import AdminDashboard from "./assets/components/AdminDashboard";
import PsychologistDashboard from "./assets/components/PsychologistDashboard";
import TeacherDashboard from "./assets/components/TeacherDashboard";
import Forbidden from "./assets/components/Forbidden";

export default function App() {
  return (
    <AuthProvider>
      <Router>
        <Box minH="100vh" bg="linear-gradient(to right, #6B46C1, #000000)" color="white">
          <AppNav />
          <Routes>
            <Route path="/" element={<LoginForm />} />

            {/* Cualquiera logueado */}
            <Route path="/dashboard" element={
              <RequireAuth>
                <Dashboard />
              </RequireAuth>
            }/>

            {/* Exclusivo ADMIN */}
            <Route path="/dashboard/admin" element={
              <RequireAuth roles={["admin"]}>
                <AdminDashboard />
              </RequireAuth>
            }/>

            {/* Exclusivo PSYCHOLOGIST */}
            <Route path="/dashboard/psychologist" element={
              <RequireAuth roles={["psychologist"]}>
                <PsychologistDashboard />
              </RequireAuth>
            }/>

            {/* Exclusivo TEACHER */}
            <Route path="/dashboard/teacher" element={
              <RequireAuth roles={["teacher"]}>
                <TeacherDashboard />
              </RequireAuth>
            }/>

            <Route path="/forbidden" element={<Forbidden />} />
          </Routes>
        </Box>
      </Router>
    </AuthProvider>
  );
}
