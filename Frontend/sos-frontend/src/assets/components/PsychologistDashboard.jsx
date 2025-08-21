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

export default function PsychologistDashboard() {
  return (
    <Box p={8}>
      <Heading size="lg" mb={6}>Panel Psychologist</Heading>

      <SimpleGrid columns={[1,2,3]} spacing={6}>
        {[1,2,3].map(i => (
          <Card key={i}>
            <CardHeader><Heading size="md">Caso #{i}</Heading></CardHeader>
            <CardBody>
              <Text>Resumen breve del caso, última sesión, próximos pasos…</Text>
              <Button mt={4} size="sm" colorScheme="purple">Abrir expediente</Button>
            </CardBody>
          </Card>
        ))}
      </SimpleGrid>
    </Box>
  );
}
