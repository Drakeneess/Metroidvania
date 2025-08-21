import { Box, Heading, Text } from "@chakra-ui/react";

export default function Forbidden() {
  return (
    <Box p={8}>
      <Heading size="lg">403 - Sin permiso</Heading>
      <Text mt={2}>No tienes acceso a esta sección.</Text>
    </Box>
  );
}
