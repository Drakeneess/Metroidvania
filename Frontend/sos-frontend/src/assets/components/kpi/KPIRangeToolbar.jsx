// src/components/kpi/KPIRangeToolbar.jsx
import { HStack, Input, Button, ButtonGroup } from "@chakra-ui/react";

const toISO = (d) => d.toISOString().slice(0,10);
const daysAgo = (n) => {
  const d = new Date(); d.setDate(d.getDate()-n); return toISO(d);
};

export default function KPIRangeToolbar({ from, to, setFrom, setTo, onApply, presets = true }) {
  return (
    <HStack mb={4} spacing={3} wrap="wrap">
      <Input type="date" value={from || ""} onChange={(e)=>setFrom(e.target.value)} w="auto" />
      <Input type="date" value={to || ""} onChange={(e)=>setTo(e.target.value)} w="auto" />
      <Button colorScheme="teal" onClick={onApply}>Aplicar</Button>
      {presets && (
        <ButtonGroup variant="outline" isAttached>
          <Button onClick={() => { setFrom(daysAgo(7)); setTo(toISO(new Date())); }}>7d</Button>
          <Button onClick={() => { setFrom(daysAgo(30)); setTo(toISO(new Date())); }}>30d</Button>
          <Button onClick={() => { setFrom(daysAgo(90)); setTo(toISO(new Date())); }}>90d</Button>
        </ButtonGroup>
      )}
    </HStack>
  );
}
