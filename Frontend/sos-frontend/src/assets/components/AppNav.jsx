import { Flex, Box, Button, HStack, Text } from "@chakra-ui/react";
import { Link as RouterLink } from "react-router-dom";
import { useAuth } from "../auth/AuthContext";

export default function AppNav() {
  const { user, logout } = useAuth();
  if (!user) return null;

  return (
    <Flex as="nav" p={4} justify="space-between" align="center" bg="blackAlpha.600">
      <HStack spacing={4}>
        <Text fontWeight="bold">Shadow of Souls</Text>
        <Button as={RouterLink} to="/dashboard" size="sm" variant="ghost">Inicio</Button>

        {/* Enlaces visibles solo si el rol coincide */}
        {user.role === "admin" && (
          <Button as={RouterLink} to="/dashboard/admin" size="sm" variant="ghost">Admin</Button>
        )}
        {user.role === "psychologist" && (
          <Button as={RouterLink} to="/dashboard/psychologist" size="sm" variant="ghost">Psychologist</Button>
        )}
        {user.role === "teacher" && (
          <Button as={RouterLink} to="/dashboard/teacher" size="sm" variant="ghost">Teacher</Button>
        )}
      </HStack>

      <HStack spacing={3}>
        <Text fontSize="sm" opacity={0.8}>{user.full_name} — {user.role}</Text>
        <Button size="sm" colorScheme="red" onClick={logout}>Cerrar sesión</Button>
      </HStack>
    </Flex>
  );
}
