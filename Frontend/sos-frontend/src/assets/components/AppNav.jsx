import { Flex, Box, Button, HStack, Text, Spacer } from "@chakra-ui/react";
import { Link as RouterLink, useLocation } from "react-router-dom";
import { useAuth } from "../auth/AuthContext";

function NavButton({ to, children, exact = false }) {
  const location = useLocation();
  const active = exact ? location.pathname === to : location.pathname.startsWith(to);

  return (
    <Button
      as={RouterLink}
      to={to}
      size="sm"
      variant="ghost"
      borderRadius="full"
      px={4}
      color={active ? "white" : "whiteAlpha.800"}
      bg={active ? "whiteAlpha.200" : "transparent"}
      _hover={{ bg: "whiteAlpha.300", color: "white" }}
      _active={{ transform: "scale(0.98)" }}
      _focusVisible={{ boxShadow: "0 0 0 2px rgba(159,122,234,0.6)" }} // morado chakra-ish
      transition="all 0.15s ease-out"
    >
      {children}
    </Button>
  );
}

export default function AppNav() {
  const { user, logout } = useAuth();
  if (!user) return null;

  return (
    <Box
      as="header"
      position="sticky"
      top="0"
      zIndex="100"
      backdropFilter="blur(10px)"
      bg="blackAlpha.600"
      borderBottom="1px solid"
      borderColor="whiteAlpha.200"
    >
      <Flex
        as="nav"
        maxW="7xl"
        mx="auto"
        w="100%"
        px={{ base: 3, md: 6 }}
        py={{ base: 3, md: 4 }}
        align="center"
        color="white"
        gap={3}
        wrap="wrap"
      >
        {/* Brand */}
        <Text
          color="white"
          bgGradient="linear(to-r, pink.300, purple.300)"
          bgClip="text"
        >
          Shadow of Souls
        </Text>

        <Spacer />

        {/* Links (se apilan en pantallas muy pequeñas) */}
        <HStack spacing={{ base: 1, md: 2 }} wrap="wrap">
          <NavButton to="/dashboard" exact>
            Inicio
          </NavButton>

          {user.role === "admin" && (
            <NavButton to="/dashboard/admin">Administrador</NavButton>
          )}
          {user.role === "psychologist" && (
            <NavButton to="/dashboard/psychologist">Psicologo</NavButton>
          )}
          {user.role === "teacher" && (
            <NavButton to="/dashboard/teacher">Profesor</NavButton>
          )}
        </HStack>

        <Spacer />

        {/* User + Logout */}
        <HStack spacing={3}>
          <Text
            fontSize="sm"
            opacity={0.9}
            noOfLines={1}
            maxW={{ base: "140px", md: "260px" }}
          >
            {user.full_name} — {user.role}
          </Text>
          <Button
            size="sm"
            colorScheme="red"
            onClick={logout}
            _focusVisible={{ boxShadow: "0 0 0 2px rgba(229,62,62,0.6)" }}
          >
            Cerrar sesión
          </Button>
        </HStack>
      </Flex>
    </Box>
  );
}
