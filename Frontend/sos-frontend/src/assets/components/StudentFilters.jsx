import { Stack, Box, Text, Input, Button } from "@chakra-ui/react";

export default function StudentFilters({ filters, setFilters, onApply, onClear }) {
  const handleChange = (field) => (e) =>
    setFilters({ ...filters, [field]: e.target.value });

  const inputProps = {
    bg: "white",
    borderColor: "gray.300",
    _placeholder: { color: "gray.500" },
    _hover: { borderColor: "gray.400" },
    _focus: { borderColor: "blue.400", boxShadow: "0 0 0 1px var(--chakra-colors-blue-400)" }
  };

  return (
    <Stack direction="row" spacing={4} mb={4} flexWrap="wrap" align="end">
      <Box>
        <Text fontSize="sm" color="gray.700" mb={1}>ID</Text>
        <Input placeholder="Buscar ID" value={filters.searchId || ""} onChange={handleChange("searchId")} {...inputProps} />
      </Box>

      <Box>
        <Text fontSize="sm" color="gray.700" mb={1}>Fecha desde</Text>
        <Input type="date" value={filters.startDate || ""} onChange={handleChange("startDate")} {...inputProps} />
      </Box>

      <Box>
        <Text fontSize="sm" color="gray.700" mb={1}>Fecha hasta</Text>
        <Input type="date" value={filters.endDate || ""} onChange={handleChange("endDate")} {...inputProps} />
      </Box>

      <Box>
        <Text fontSize="sm" color="gray.700" mb={1}>Edad mínima</Text>
        <Input type="number" inputMode="numeric" value={filters.minAge || ""} onChange={handleChange("minAge")} {...inputProps} />
      </Box>

      <Box>
        <Text fontSize="sm" color="gray.700" mb={1}>Edad máxima</Text>
        <Input type="number" inputMode="numeric" value={filters.maxAge || ""} onChange={handleChange("maxAge")} {...inputProps} />
      </Box>

      <Stack direction="row" spacing={2}>
        <Button colorScheme="blue" onClick={onApply}>Filtrar</Button>
        {onClear && <Button variant="outline" onClick={onClear}>Limpiar</Button>}
      </Stack>
    </Stack>
  );
}
