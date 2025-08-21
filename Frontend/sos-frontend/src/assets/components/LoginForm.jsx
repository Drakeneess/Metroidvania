import { useState } from "react";
import { Box, Button, Input, VStack, Heading, Text } from "@chakra-ui/react";
import axios from "axios";
import { useNavigate } from "react-router-dom";
import { useAuth } from "../auth/AuthContext";

export default function LoginForm() {
  const [email, setEmail]       = useState("");
  const [password, setPassword] = useState("");
  const [message, setMessage]   = useState("");
  const navigate = useNavigate();
  const { login } = useAuth();

  const handleLogin = async () => {
    try {
      const res = await axios.post("http://localhost:4000/auth/login", { email, password });
      login(res.data); // guarda token + user en contexto y localStorage
      // Redirección según rol
      const role = res.data.user.role; // "admin" | "psychologist" | "teacher" | ...
      if (role === "admin")        navigate("/dashboard/admin");
      else if (role === "gabinete" || role === "psychologist") navigate("/dashboard/gabinete");
      else if (role === "teacher") navigate("/dashboard/teacher");
      else                         navigate("/dashboard"); // fallback
    } catch (err) {
      setMessage(err.response?.data?.error || "Error al iniciar sesión");
    }
  };

  return (
    <Box maxW="md" mx="auto" mt="10" p="6" borderWidth="1px" borderRadius="lg">
      <VStack spacing={4}>
        <Heading size="lg">Login</Heading>
        <Input placeholder="Correo electrónico" value={email} onChange={(e) => setEmail(e.target.value)} />
        <Input placeholder="Contraseña" type="password" value={password} onChange={(e) => setPassword(e.target.value)} />
        <Button colorScheme="teal" onClick={handleLogin}>Iniciar sesión</Button>
        {message && <Text>{message}</Text>}
      </VStack>
    </Box>
  );
}
