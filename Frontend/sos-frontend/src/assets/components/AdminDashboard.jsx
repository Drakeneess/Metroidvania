import {
  Box,
  Heading,
  SimpleGrid,
  VStack,
  Button,
} from "@chakra-ui/react";

import {
  Stat,
  StatLabel,
  StatNumber,
  StatHelpText,
  StatArrow,
} from "@chakra-ui/stat";


export default function AdminDashboard() {
  return (
    <Box p={8}>
      <Heading size="lg" mb={6}>Panel Admin</Heading>

      <SimpleGrid columns={[1, 3]} spacing={6} mb={8}>
        <Stat>
          <StatLabel>Usuarios</StatLabel>
          <StatNumber>120</StatNumber>
        </Stat>
        <Stat>
          <StatLabel>Reportes</StatLabel>
          <StatNumber>45</StatNumber>
        </Stat>
        <Stat>
          <StatLabel>Sesiones</StatLabel>
          <StatNumber>32</StatNumber>
        </Stat>
      </SimpleGrid>

      <VStack align="start" spacing={3}>
        <Button colorScheme="teal">Crear usuario</Button>
        <Button variant="outline">Asignar rol</Button>
        <Button variant="outline">Suspender/Activar cuenta</Button>
        <Button variant="outline">Exportar reporte</Button>
      </VStack>
    </Box>
  );
}
