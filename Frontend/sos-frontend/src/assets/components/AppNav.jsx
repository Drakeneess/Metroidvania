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
  Badge,
  Avatar,
  Menu,
  MenuButton,
  MenuList,
  MenuItem,
  MenuDivider,
} from "@chakra-ui/react";
import { Link as RouterLink, useLocation } from "react-router-dom";
import { useAuth } from "../auth/AuthContext";
import { HamburgerIcon, CloseIcon, ChevronDownIcon } from "@chakra-ui/icons";

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
      color={active ? "white" : "gray.300"}
      bg={active ? "rgba(169,112,255,0.24)" : "transparent"}
      border="1px solid"
      borderColor={active ? "rgba(169,112,255,0.35)" : "transparent"}
      _hover={{
        bg: active ? "rgba(169,112,255,0.30)" : "whiteAlpha.200",
        color: "white",
      }}
      _active={{
        transform: "scale(0.98)",
      }}
      transition="all 0.15s ease-out"
      onClick={onClick}
    >
      {children}
    </Button>
  );
}

function getRoleLabel(role) {
  if (role === "admin") return "Administrador";
  if (role === "psychologist") return "Psicólogo";
  if (role === "teacher") return "Docente";
  return "Usuario";
}

function getUserName(user) {
  return user?.full_name || user?.username || user?.email || "Usuario";
}

export default function AppNav() {
  const { user, logout } = useAuth();
  const { isOpen, onToggle, onClose } = useDisclosure();

  if (!user) return null;

  const userName = getUserName(user);
  const roleLabel = getRoleLabel(user.role);

  const links = [
    {
      to: "/dashboard",
      label: "Inicio",
      exact: true,
      roles: ["admin", "psychologist", "teacher"],
    },
    {
      to: "/dashboard/admin",
      label: "Administrador",
      roles: ["admin"],
    },
    {
      to: "/dashboard/psychologist",
      label: "Psicólogo",
      roles: ["admin", "psychologist"],
    },
    {
      to: "/dashboard/teacher",
      label: "Docente",
      roles: ["admin", "teacher"],
    },
  ].filter((link) => link.roles.includes(user.role));

  return (
    <Box
      as="header"
      position="sticky"
      top="0"
      zIndex="100"
      backdropFilter="blur(16px) saturate(160%)"
      bg="rgba(15, 12, 41, 0.78)"
      borderBottom="1px solid"
      borderColor="rgba(214,188,250,0.16)"
      boxShadow="0 8px 28px rgba(0,0,0,0.22)"
    >
      <Flex
        maxW="1440px"
        mx="auto"
        px={{ base: 4, md: 8 }}
        py={{ base: 3, md: 3.5 }}
        align="center"
        color="white"
      >
        {/* Brand */}
        <HStack spacing={3}>
          <Box
            w="36px"
            h="36px"
            borderRadius="xl"
            bg="linear-gradient(135deg, #A970FF, #6B35C8)"
            boxShadow="0 0 22px rgba(169,112,255,0.38)"
            display="flex"
            alignItems="center"
            justifyContent="center"
            fontWeight="900"
            color="white"
          >
            S
          </Box>

          <Box>
            <Text
              fontWeight="900"
              fontSize={{ base: "md", md: "lg" }}
              bgGradient="linear(to-r, brand.100, brand.300, calm)"
              bgClip="text"
              letterSpacing="-0.03em"
              lineHeight="1"
            >
              Shadow of Souls
            </Text>

            <Text
              display={{ base: "none", md: "block" }}
              color="gray.500"
              fontSize="xs"
              fontWeight="600"
              mt={1}
            >
              Administración y seguimiento
            </Text>
          </Box>
        </HStack>

        <Spacer />

        {/* Desktop links */}
        <HStack
          display={{ base: "none", md: "flex" }}
          spacing={2}
          align="center"
        >
          {links.map((link) => (
            <NavButton key={link.to} to={link.to} exact={link.exact}>
              {link.label}
            </NavButton>
          ))}

          <Box h="28px" w="1px" bg="whiteAlpha.200" mx={2} />

          <Menu placement="bottom-end">
            <MenuButton
              as={Button}
              variant="ghost"
              size="sm"
              borderRadius="full"
              px={2}
              rightIcon={<ChevronDownIcon />}
              _hover={{
                bg: "whiteAlpha.200",
              }}
              _active={{
                bg: "whiteAlpha.300",
              }}
            >
              <HStack spacing={2}>
                <Avatar
                  size="xs"
                  name={userName}
                  bg="brand.500"
                  color="white"
                />

                <Text
                  maxW="140px"
                  noOfLines={1}
                  color="gray.100"
                  fontWeight="700"
                  fontSize="sm"
                >
                  {userName}
                </Text>
              </HStack>
            </MenuButton>

            <MenuList
              bg="rgba(35, 41, 70, 0.98)"
              borderColor="soul.border"
              color="gray.100"
              boxShadow="soul"
              backdropFilter="blur(18px)"
              p={2}
            >
              <Box px={3} py={2}>
                <Text fontWeight="800" color="white" noOfLines={1}>
                  {userName}
                </Text>

                <Badge
                  mt={2}
                  px={2}
                  py={1}
                  borderRadius="full"
                  bg="rgba(169,112,255,0.18)"
                  color="brand.100"
                  border="1px solid"
                  borderColor="rgba(169,112,255,0.32)"
                >
                  {roleLabel}
                </Badge>
              </Box>

              <MenuDivider borderColor="whiteAlpha.200" />

              <MenuItem
                onClick={logout}
                borderRadius="lg"
                bg="transparent"
                color="danger"
                fontWeight="700"
                _hover={{
                  bg: "rgba(239,71,111,0.12)",
                }}
              >
                Cerrar sesión
              </MenuItem>
            </MenuList>
          </Menu>
        </HStack>

        {/* Mobile toggle */}
        <IconButton
          display={{ base: "flex", md: "none" }}
          aria-label={isOpen ? "Cerrar menú" : "Abrir menú"}
          icon={isOpen ? <CloseIcon /> : <HamburgerIcon />}
          variant="ghost"
          color="white"
          borderRadius="xl"
          _hover={{
            bg: "whiteAlpha.200",
          }}
          onClick={onToggle}
        />
      </Flex>

      {/* Mobile menu */}
      <Collapse in={isOpen} animateOpacity>
        <VStack
          bg="rgba(15, 12, 41, 0.94)"
          align="stretch"
          py={4}
          px={4}
          spacing={2}
          display={{ md: "none" }}
          borderTop="1px solid"
          borderColor="whiteAlpha.200"
        >
          <Box
            px={3}
            py={3}
            borderRadius="xl"
            bg="rgba(255,255,255,0.05)"
            border="1px solid"
            borderColor="whiteAlpha.100"
            mb={2}
          >
            <HStack>
              <Avatar size="sm" name={userName} bg="brand.500" color="white" />

              <Box minW={0}>
                <Text color="white" fontWeight="800" noOfLines={1}>
                  {userName}
                </Text>

                <Text color="brand.200" fontSize="sm" fontWeight="600">
                  {roleLabel}
                </Text>
              </Box>
            </HStack>
          </Box>

          {links.map((link) => (
            <NavButton
              key={link.to}
              to={link.to}
              exact={link.exact}
              onClick={onClose}
            >
              {link.label}
            </NavButton>
          ))}

          <Button
            size="sm"
            variant="outline"
            borderColor="rgba(239,71,111,0.38)"
            color="danger"
            fontWeight="800"
            mt={2}
            _hover={{
              bg: "rgba(239,71,111,0.12)",
            }}
            onClick={() => {
              onClose();
              logout();
            }}
          >
            Cerrar sesión
          </Button>
        </VStack>
      </Collapse>
    </Box>
  );
}