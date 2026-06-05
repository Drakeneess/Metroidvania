// src/assets/components/NotFound.jsx

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
  Code,
} from "@chakra-ui/react";
import { Link as RouterLink, useLocation } from "react-router-dom";
import { FiCompass, FiHome, FiArrowLeft, FiMap } from "react-icons/fi";

export default function NotFound() {
  const location = useLocation();

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
        maxW="760px"
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
          bg: "radial-gradient(circle at top right, rgba(169,112,255,0.22), transparent 38%)",
          pointerEvents: "none",
        }}
        _after={{
          content: '""',
          position: "absolute",
          left: "-80px",
          bottom: "-80px",
          w: "220px",
          h: "220px",
          bg: "radial-gradient(circle, rgba(0,187,249,0.13), transparent 65%)",
          pointerEvents: "none",
        }}
      >
        <VStack position="relative" spacing={6}>
          <Flex
            w="76px"
            h="76px"
            align="center"
            justify="center"
            borderRadius="2xl"
            bg="rgba(169, 112, 255, 0.16)"
            border="1px solid"
            borderColor="rgba(169, 112, 255, 0.32)"
            color="brand.100"
            boxShadow="glow"
          >
            <Icon as={FiCompass} boxSize={9} />
          </Flex>

          <Box>
            <HStack justify="center" spacing={2} mb={3} flexWrap="wrap">
              <Badge
                px={3}
                py={1}
                borderRadius="full"
                bg="rgba(169, 112, 255, 0.18)"
                color="brand.100"
                border="1px solid"
                borderColor="rgba(169, 112, 255, 0.32)"
              >
                404
              </Badge>

              <Badge
                px={3}
                py={1}
                borderRadius="full"
                bg="rgba(0, 187, 249, 0.14)"
                color="calm"
                border="1px solid"
                borderColor="rgba(0, 187, 249, 0.26)"
              >
                Ruta no encontrada
              </Badge>
            </HStack>

            <Heading
              size={{ base: "lg", md: "xl" }}
              color="white"
              letterSpacing="-0.03em"
            >
              Esta sección no existe
            </Heading>

            <Text color="gray.400" mt={3} fontSize={{ base: "sm", md: "md" }}>
              La ruta solicitada no está registrada en el sistema. Puede que el
              enlace esté roto, la sección haya cambiado o alguien haya escrito
              una URL como si estuviera invocando magia antigua.
            </Text>
          </Box>

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
              <Icon as={FiMap} color="brand.200" boxSize={5} />

              <Box minW={0}>
                <Text color="gray.400" fontSize="sm">
                  Ruta solicitada
                </Text>

                <Code
                  mt={1}
                  display="block"
                  bg="rgba(0,0,0,0.24)"
                  color="brand.100"
                  borderRadius="md"
                  px={3}
                  py={2}
                  whiteSpace="normal"
                  wordBreak="break-word"
                >
                  {location.pathname}
                </Code>
              </Box>
            </HStack>
          </Box>

          <HStack spacing={3} flexWrap="wrap" justify="center">
            <Button as={RouterLink} to="/dashboard" leftIcon={<FiHome />}>
              Volver al inicio
            </Button>

            <Button
              variant="outline"
              leftIcon={<FiArrowLeft />}
              onClick={() => window.history.back()}
            >
              Regresar
            </Button>
          </HStack>

          <Text color="gray.600" fontSize="xs">
            Error 404 · Shadow of Souls no encontró esta ruta en el mapa.
          </Text>
        </VStack>
      </Box>
    </Flex>
  );
}