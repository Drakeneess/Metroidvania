// src/components/kpi/KPIRangeToolbar.jsx

import {
  Box,
  Button,
  ButtonGroup,
  FormControl,
  FormLabel,
  HStack,
  Input,
  Text,
  VStack,
} from "@chakra-ui/react";

const toISO = (date) => date.toISOString().slice(0, 10);

const daysAgo = (days) => {
  const date = new Date();
  date.setDate(date.getDate() - days);
  return toISO(date);
};

export default function KPIRangeToolbar({
  from,
  to,
  setFrom,
  setTo,
  onApply,
  presets = true,
  loading = false,
}) {
  const today = toISO(new Date());

  const applyPreset = (days) => {
    setFrom(daysAgo(days));
    setTo(today);
  };

  const clearRange = () => {
    setFrom(null);
    setTo(null);
  };

  return (
    <VStack align="stretch" spacing={4}>
      <HStack
        spacing={4}
        align="end"
        flexWrap="wrap"
      >
        <FormControl w={{ base: "100%", md: "220px" }}>
          <FormLabel color="gray.400" fontSize="sm">
            Desde
          </FormLabel>

          <Input
            type="date"
            value={from || ""}
            onChange={(event) => setFrom(event.target.value)}
          />
        </FormControl>

        <FormControl w={{ base: "100%", md: "220px" }}>
          <FormLabel color="gray.400" fontSize="sm">
            Hasta
          </FormLabel>

          <Input
            type="date"
            value={to || ""}
            onChange={(event) => setTo(event.target.value)}
          />
        </FormControl>

        <Button
          onClick={onApply}
          isLoading={loading}
          minW={{ base: "100%", md: "120px" }}
        >
          Aplicar
        </Button>

        <Button
          variant="ghost"
          onClick={clearRange}
          minW={{ base: "100%", md: "100px" }}
        >
          Limpiar
        </Button>
      </HStack>

      {presets && (
        <Box>
          <Text color="gray.500" fontSize="xs" mb={2}>
            Rangos rápidos
          </Text>

          <ButtonGroup
            variant="outline"
            spacing={2}
            flexWrap="wrap"
            gap={2}
          >
            <Button size="sm" onClick={() => applyPreset(7)}>
              Últimos 7 días
            </Button>

            <Button size="sm" onClick={() => applyPreset(30)}>
              Últimos 30 días
            </Button>

            <Button size="sm" onClick={() => applyPreset(90)}>
              Últimos 90 días
            </Button>
          </ButtonGroup>
        </Box>
      )}
    </VStack>
  );
}