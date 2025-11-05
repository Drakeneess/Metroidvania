import {
  Box,
  Flex,
  Button,
  HStack,
  Text,
  Spacer,
  IconButton,
  Collapse,
  VStack,
  useDisclosure,
} from "@chakra-ui/react";
import { Link as RouterLink, useLocation } from "react-router-dom";
import { useAuth } from "../auth/AuthContext";
import { HamburgerIcon, CloseIcon } from "@chakra-ui/icons";

function NavButton({ to, children, exact = false, onClick }) {
  const location = useLocation();
  const active = exact
    ? location.pathname === to
    : location.pathname.startsWith(to);

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
      transition="all 0.15s ease-out"
      onClick={onClick}
    >
      {children}
    </Button>
  );
}

export default function AppNav() {
  const { user, logout } = useAuth();
  const { isOpen, onToggle } = useDisclosure();
  if (!user) return null;

  return (
    <Box
      as="header"
      position="sticky"
      top="0"
      zIndex="100"
      backdropFilter="blur(10px)"
      bg="blackAlpha.700"
      borderBottom="1px solid"
      borderColor="whiteAlpha.200"
    >
      <Flex
        maxW="7xl"
        mx="auto"
        px={{ base: 4, md: 6 }}
        py={{ base: 3, md: 4 }}
        align="center"
        color="white"
      >
        {/* Brand */}
        <Text
          fontWeight="bold"
          fontSize="lg"
          bgGradient="linear(to-r, pink.300, purple.300)"
          bgClip="text"
        >
          Shadow of Souls
        </Text>

        <Spacer />

        {/* Desktop links */}
        <HStack
          display={{ base: "none", md: "flex" }}
          spacing={2}
          align="center"
        >
          <NavButton to="/dashboard" exact>
            Inicio
          </NavButton>
          {user.role === "admin" && (
            <NavButton to="/dashboard/admin">Administrador</NavButton>
          )}
          {user.role === "psychologist" && (
            <NavButton to="/dashboard/psychologist">Psicólogo</NavButton>
          )}
          {user.role === "teacher" && (
            <NavButton to="/dashboard/teacher">Profesor</NavButton>
          )}
          <Button size="sm" colorScheme="red" onClick={logout}>
            Salir
          </Button>
        </HStack>

        {/* Mobile toggle */}
        <IconButton
          display={{ base: "flex", md: "none" }}
          aria-label="Abrir menú"
          icon={isOpen ? <CloseIcon /> : <HamburgerIcon />}
          variant="ghost"
          color="white"
          onClick={onToggle}
        />
      </Flex>

      {/* Mobile menu */}
      <Collapse in={isOpen} animateOpacity>
        <VStack
          bg="blackAlpha.800"
          align="stretch"
          py={3}
          px={4}
          spacing={1}
          display={{ md: "none" }}
        >
          <NavButton to="/dashboard" exact onClick={onToggle}>
            Inicio
          </NavButton>
          {user.role === "admin" && (
            <NavButton to="/dashboard/admin" onClick={onToggle}>
              Administrador
            </NavButton>
          )}
          {user.role === "psychologist" && (
            <NavButton to="/dashboard/psychologist" onClick={onToggle}>
              Psicólogo
            </NavButton>
          )}
          {user.role === "teacher" && (
            <NavButton to="/dashboard/teacher" onClick={onToggle}>
              Profesor
            </NavButton>
          )}
          <Button size="sm" colorScheme="red" onClick={logout}>
            Cerrar sesión
          </Button>
        </VStack>
      </Collapse>
    </Box>
  );
}
