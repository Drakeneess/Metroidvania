// src/assets/components/LoginForm.jsx

import { useState } from "react";
import {
  Box,
  Button,
  Input,
  VStack,
  Heading,
  Text,
  Flex,
  FormControl,
  FormLabel,
  Alert,
  AlertIcon,
  Badge,
  HStack,
  Icon,
  InputGroup,
  InputLeftElement,
  InputRightElement,
  IconButton,
} from "@chakra-ui/react";
import {
  FiLock,
  FiMail,
  FiShield,
  FiEye,
  FiEyeOff,
} from "react-icons/fi";
import { useNavigate } from "react-router-dom";
import { useAuth } from "../auth/AuthContext";
import { api } from "../../lib/axiosInstance";

export default function LoginForm() {
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");

  const [showPassword, setShowPassword] = useState(false);
  const [message, setMessage] = useState("");
  const [loading, setLoading] = useState(false);

  const navigate = useNavigate();
  const { login } = useAuth();

  const handleLogin = async (event) => {
    event?.preventDefault();

    if (!email.trim() || !password.trim()) {
      setMessage("Ingresa tu correo y contraseña.");
      return;
    }

    setLoading(true);
    setMessage("");

    try {
      const res = await api.post("/auth/login", {
        email: email.trim(),
        password,
      });

      await login(res.data);

      const role = res.data.user.role;

      if (role === "admin") navigate("/dashboard/admin");
      else if (role === "psychologist") navigate("/dashboard/psychologist");
      else if (role === "teacher") navigate("/dashboard/teacher");
      else navigate("/dashboard");
    } catch (err) {
      setMessage(err?.response?.data?.error || "Error al iniciar sesión");
    } finally {
      setLoading(false);
    }
  };

  return (
    <Flex
      minH="100vh"
      align="center"
      justify="center"
      px={{ base: 4, md: 8 }}
      py={8}
      position="relative"
      overflow="hidden"
      bg="transparent"
    >
      <Box
        position="absolute"
        inset={0}
        bg="radial-gradient(circle at top left, rgba(169,112,255,0.30), transparent 34%), radial-gradient(circle at bottom right, rgba(0,187,249,0.14), transparent 32%)"
        pointerEvents="none"
      />

      <Box
        position="relative"
        maxW="460px"
        w="full"
        p={{ base: 6, md: 8 }}
        borderRadius="2xl"
        boxShadow="soul"
        bg="rgba(35, 41, 70, 0.86)"
        backdropFilter="blur(18px)"
        border="1px solid"
        borderColor="soul.border"
        color="white"
        overflow="hidden"
        _before={{
          content: '""',
          position: "absolute",
          inset: 0,
          bg: "radial-gradient(circle at top right, rgba(169,112,255,0.22), transparent 38%)",
          pointerEvents: "none",
        }}
      >
        <VStack
          as="form"
          onSubmit={handleLogin}
          position="relative"
          spacing={6}
        >
          <Flex
            w="58px"
            h="58px"
            align="center"
            justify="center"
            borderRadius="2xl"
            bg="rgba(169, 112, 255, 0.16)"
            border="1px solid"
            borderColor="rgba(169, 112, 255, 0.32)"
            color="brand.100"
            boxShadow="glow"
          >
            <Icon as={FiShield} boxSize={7} />
          </Flex>

          <Box textAlign="center">
            <HStack justify="center" mb={3}>
              <Badge
                px={3}
                py={1}
                borderRadius="full"
                bg="rgba(169, 112, 255, 0.18)"
                color="brand.100"
                border="1px solid"
                borderColor="rgba(169, 112, 255, 0.32)"
              >
                Shadow of Souls
              </Badge>
            </HStack>

            <Heading size="lg" color="white" letterSpacing="-0.03em">
              Iniciar sesión
            </Heading>

            <Text color="gray.400" mt={2} fontSize="sm">
              Accede al panel de seguimiento institucional.
            </Text>
          </Box>

          {message && (
            <Alert
              status="error"
              borderRadius="xl"
              bg="rgba(239, 71, 111, 0.12)"
              border="1px solid"
              borderColor="rgba(239, 71, 111, 0.28)"
              color="gray.100"
              py={3}
            >
              <AlertIcon />
              <Text fontSize="sm">{message}</Text>
            </Alert>
          )}

          <VStack spacing={4} w="full">
            <FormControl>
              <FormLabel color="gray.400" fontSize="sm">
                Correo electrónico
              </FormLabel>

              <InputGroup>
                <InputLeftElement pointerEvents="none" color="brand.200">
                  <FiMail />
                </InputLeftElement>

                <Input
                  placeholder="correo@ejemplo.com"
                  type="email"
                  value={email}
                  onChange={(event) => setEmail(event.target.value)}
                  autoComplete="email"
                  pl="42px"
                />
              </InputGroup>
            </FormControl>

            <FormControl>
              <FormLabel color="gray.400" fontSize="sm">
                Contraseña
              </FormLabel>

              <InputGroup>
                <InputLeftElement pointerEvents="none" color="brand.200">
                  <FiLock />
                </InputLeftElement>

                <Input
                  placeholder="Ingresa tu contraseña"
                  type={showPassword ? "text" : "password"}
                  value={password}
                  onChange={(event) => setPassword(event.target.value)}
                  autoComplete="current-password"
                  pl="42px"
                  pr="46px"
                />

                <InputRightElement>
                  <IconButton
                    aria-label={
                      showPassword
                        ? "Ocultar contraseña"
                        : "Mostrar contraseña"
                    }
                    icon={showPassword ? <FiEyeOff /> : <FiEye />}
                    size="sm"
                    variant="ghost"
                    color="gray.400"
                    _hover={{
                      color: "brand.100",
                      bg: "whiteAlpha.200",
                    }}
                    _active={{
                      bg: "whiteAlpha.300",
                    }}
                    onClick={() =>
                      setShowPassword((currentValue) => !currentValue)
                    }
                  />
                </InputRightElement>
              </InputGroup>
            </FormControl>
          </VStack>

          <Button
            type="submit"
            w="full"
            size="lg"
            isLoading={loading}
            loadingText="Validando..."
            leftIcon={<FiShield />}
          >
            Iniciar sesión
          </Button>

          <Text color="gray.500" fontSize="xs" textAlign="center">
            Sistema de administración y seguimiento de indicadores derivados del
            videojuego.
          </Text>
        </VStack>
      </Box>
    </Flex>
  );
}