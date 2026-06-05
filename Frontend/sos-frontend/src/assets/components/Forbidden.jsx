// src/assets/components/Forbidden.jsx

import {
  Box,
  Button,
  Heading,
  Text,
  VStack,
  HStack,
  Badge,
  Icon,
  Flex,
} from "@chakra-ui/react";
import { Link as RouterLink } from "react-router-dom";
import { FiLock, FiHome, FiAlertTriangle } from "react-icons/fi";
import { useAuth } from "../auth/AuthContext";

function getRoleLabel(role) {
  if (role === "admin") return "Administrador";
  if (role === "psychologist") return "Psicólogo";
  if (role === "teacher") return "Docente";
  return "Usuario";
}

export default function Forbidden() {
  const { user } = useAuth();

  return (
    <Flex
      minH="calc(100vh - 80px)"
      align="center"
      justify="center"
      px={{ base: 4, md: 8 }}
      py={{ base: 8, md: 12 }}
    >
      <Box
        position="relative"
        maxW="720px"
        w="full"
        p={{ base: 6, md: 8 }}
        bg="rgba(35, 41, 70, 0.84)"
        border="1px solid"
        borderColor="soul.border"
        borderRadius="2xl"
        boxShadow="soul"
        backdropFilter="blur(18px)"
        overflow="hidden"
        textAlign="center"
        _before={{
          content: '""',
          position: "absolute",
          inset: 0,
          bg: "radial-gradient(circle at top right, rgba(239,71,111,0.18), transparent 38%)",
          pointerEvents: "none",
        }}
      >
        <VStack position="relative" spacing={6}>
          <Flex
            w="72px"
            h="72px"
            align="center"
            justify="center"
            borderRadius="2xl"
            bg="rgba(239, 71, 111, 0.14)"
            border="1px solid"
            borderColor="rgba(239, 71, 111, 0.32)"
            color="danger"
            boxShadow="0 0 28px rgba(239,71,111,0.22)"
          >
            <Icon as={FiLock} boxSize={8} />
          </Flex>

          <Box>
            <HStack justify="center" spacing={2} mb={3} flexWrap="wrap">
              <Badge
                px={3}
                py={1}
                borderRadius="full"
                bg="rgba(239, 71, 111, 0.14)"
                color="danger"
                border="1px solid"
                borderColor="rgba(239, 71, 111, 0.32)"
              >
                403
              </Badge>

              <Badge
                px={3}
                py={1}
                borderRadius="full"
                bg="rgba(169, 112, 255, 0.18)"
                color="brand.100"
                border="1px solid"
                borderColor="rgba(169, 112, 255, 0.32)"
              >
                Acceso restringido
              </Badge>
            </HStack>

            <Heading
              size={{ base: "lg", md: "xl" }}
              color="white"
              letterSpacing="-0.03em"
            >
              No tienes permiso para entrar aquí
            </Heading>

            <Text color="gray.400" mt={3} fontSize={{ base: "sm", md: "md" }}>
              Esta sección requiere un rol distinto al de tu cuenta actual.
              El sistema bloqueó el acceso para proteger la información del panel.
            </Text>
          </Box>

          {user && (
            <Box
              w="full"
              p={4}
              bg="rgba(255,255,255,0.045)"
              border="1px solid"
              borderColor="soul.softBorder"
              borderRadius="xl"
              textAlign="left"
            >
              <HStack spacing={3} align="flex-start">
                <Icon as={FiAlertTriangle} color="warning" boxSize={5} />

                <Box>
                  <Text color="gray.400" fontSize="sm">
                    Sesión actual
                  </Text>

                  <Text color="white" fontWeight="800">
                    {user.full_name || user.username || user.email || "Usuario"}
                  </Text>

                  <Text color="brand.200" fontSize="sm" mt={1}>
                    Rol: {getRoleLabel(user.role)}
                  </Text>
                </Box>
              </HStack>
            </Box>
          )}

          <HStack spacing={3} flexWrap="wrap" justify="center">
            <Button
              as={RouterLink}
              to="/dashboard"
              leftIcon={<FiHome />}
            >
              Volver al inicio
            </Button>

            <Button
              variant="outline"
              onClick={() => window.history.back()}
            >
              Regresar
            </Button>
          </HStack>

          <Text color="gray.600" fontSize="xs">
            Si crees que esto es un error, revisa el rol asignado a tu cuenta.
          </Text>
        </VStack>
      </Box>
    </Flex>
  );
}