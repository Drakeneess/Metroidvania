// src/assets/components/StudentFilters.jsx

import {
  Box,
  Button,
  FormControl,
  FormLabel,
  Input,
  SimpleGrid,
  HStack,
} from "@chakra-ui/react";

export default function StudentFilters({
  filters,
  setFilters,
  onApply,
  onClear,
}) {
  const handleChange = (field) => (event) => {
    setFilters({
      ...filters,
      [field]: event.target.value,
    });
  };

  const handleSubmit = (event) => {
    event.preventDefault();
    onApply?.();
  };

  const clearFilters = () => {
    if (onClear) {
      onClear();
      return;
    }

    setFilters({
      searchId: "",
      startDate: "",
      endDate: "",
      minAge: "",
      maxAge: "",
    });
  };

  return (
    <Box as="form" onSubmit={handleSubmit}>
      <SimpleGrid
        columns={{ base: 1, md: 2, xl: 5 }}
        spacing={4}
        alignItems="end"
      >
        <FormControl>
          <FormLabel color="gray.400" fontSize="sm">
            ID
          </FormLabel>

          <Input
            placeholder="Buscar ID"
            value={filters.searchId || ""}
            onChange={handleChange("searchId")}
          />
        </FormControl>

        <FormControl>
          <FormLabel color="gray.400" fontSize="sm">
            Fecha desde
          </FormLabel>

          <Input
            type="date"
            value={filters.startDate || ""}
            onChange={handleChange("startDate")}
          />
        </FormControl>

        <FormControl>
          <FormLabel color="gray.400" fontSize="sm">
            Fecha hasta
          </FormLabel>

          <Input
            type="date"
            value={filters.endDate || ""}
            onChange={handleChange("endDate")}
          />
        </FormControl>

        <FormControl>
          <FormLabel color="gray.400" fontSize="sm">
            Edad mínima
          </FormLabel>

          <Input
            type="number"
            inputMode="numeric"
            placeholder="Ej. 12"
            value={filters.minAge || ""}
            onChange={handleChange("minAge")}
          />
        </FormControl>

        <FormControl>
          <FormLabel color="gray.400" fontSize="sm">
            Edad máxima
          </FormLabel>

          <Input
            type="number"
            inputMode="numeric"
            placeholder="Ej. 18"
            value={filters.maxAge || ""}
            onChange={handleChange("maxAge")}
          />
        </FormControl>
      </SimpleGrid>

      <HStack spacing={3} mt={5} justify="flex-end" flexWrap="wrap">
        <Button type="submit">
          Filtrar
        </Button>

        <Button type="button" variant="outline" onClick={clearFilters}>
          Limpiar
        </Button>
      </HStack>
    </Box>
  );
}