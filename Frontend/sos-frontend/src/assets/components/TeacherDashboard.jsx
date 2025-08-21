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

export default function TeacherDashboard() {
  return (
    <Box p={8}>
      <Heading size="lg" mb={6}>Panel Teacher</Heading>

      <Table variant="simple" size="sm">
        <Thead>
          <Tr>
            <Th>Estudiante</Th>
            <Th>Última sesión</Th>
            <Th>Estado</Th>
            <Th isNumeric>Acciones</Th>
          </Tr>
        </Thead>
        <Tbody>
          {["Ana","Luis","Mar"].map((name, idx) => (
            <Tr key={idx}>
              <Td>{name}</Td>
              <Td>2025-08-12</Td>
              <Td>Activo</Td>
              <Td isNumeric>
                <Button size="xs" variant="outline">Ver</Button>
              </Td>
            </Tr>
          ))}
        </Tbody>
      </Table>
    </Box>
  );
}
