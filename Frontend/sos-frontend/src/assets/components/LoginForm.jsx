import { useState } from "react";
import {
  Box,
  Button,
  Input,
  VStack,
  Heading,
  Text,
  Flex,
} from "@chakra-ui/react";
import { useNavigate } from "react-router-dom";
import { useAuth } from "../auth/AuthContext";
import { api } from "../../lib/axiosInstance";

export default function LoginForm() {
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [message, setMessage] = useState("");
  const navigate = useNavigate();
  const { login } = useAuth();

  const handleLogin = async () => {
    try {
      const res = await api.post("/auth/login", { email, password });
      await login(res.data);

      const role = res.data.user.role;
      if (role === "admin") navigate("/dashboard/admin");
      else if (role === "psychologist") navigate("/dashboard/psychologist");
      else if (role === "teacher") navigate("/dashboard/teacher");
      else navigate("/dashboard");
    } catch (err) {
      setMessage(err?.response?.data?.error || "Error al iniciar sesión");
    }
  };

  return (
    <Flex
  minH="100vh"
  align="center"
  justify="center"
  bgGradient="linear(to-r, purple.800, black)"
>
  <Box
    maxW="md"
    w="full"
    p="10"
    borderRadius="2xl"
    boxShadow="2xl"
    bg="rgba(255, 255, 255, 0.08)"
    backdropFilter="blur(12px)"
    border="1px solid rgba(255,255,255,0.2)"
    color="white"
  >
    <VStack spacing={8}>
      <Input
        placeholder="Correo electrónico"
        type="email"
        variant="outline"
        borderColor="purple.300"
        _hover={{ borderColor: "purple.400", boxShadow: "0 0 6px #9F7AEA" }}
        _focus={{ borderColor: "pink.400", boxShadow: "0 0 10px #D53F8C" }}
        value={email}
        onChange={(e) => setEmail(e.target.value)}
      />

      <Input
        placeholder="Contraseña"
        type="password"
        variant="outline"
        borderColor="purple.300"
        _hover={{ borderColor: "purple.400", boxShadow: "0 0 6px #9F7AEA" }}
        _focus={{ borderColor: "pink.400", boxShadow: "0 0 10px #D53F8C" }}
        value={password}
        onChange={(e) => setPassword(e.target.value)}
      />

      <Button
        w="full"
        size="lg"
        bgGradient="linear(to-r, purple.600, pink.500)"
        _hover={{
          transform: "scale(1.05)",
          bgGradient: "linear(to-r, purple.700, pink.600)",
        }}
        _active={{ transform: "scale(0.97)" }}
        transition="all 0.2s"
        color="white"
        fontWeight="bold"
        onClick={handleLogin}
      >
        Iniciar sesión
      </Button>

      {message && (
        <Text color="red.300" fontWeight="semibold" textAlign="center">
          {message}
        </Text>
      )}
    </VStack>
  </Box>
</Flex>

  );
}
