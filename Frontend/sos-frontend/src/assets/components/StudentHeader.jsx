// src/components/student-detail/StudentHeader.jsx

import {
  Box,
  Flex,
  HStack,
  Text,
  Tag,
  Tooltip,
} from "@chakra-ui/react";
import { formatDate } from "./utils";

function isRealAlert(alert) {
  const alertColor = alert?.color;
  const alertType = alert?.type?.toLowerCase?.();

  return Boolean(alertColor && alertType && alertType !== "sin alerta");
}

export default function StudentHeader({ student }) {
  if (!student) {
    return (
      <Text fontWeight="800" fontSize="lg" color="#1A202C">
        Detalle del estudiante
      </Text>
    );
  }

  const showAlert = isRealAlert(student.alert);

  return (
    <Flex
      align={{ base: "flex-start", md: "center" }}
      justify="space-between"
      direction={{ base: "column", md: "row" }}
      gap={3}
      w="100%"
      pr={{ base: 8, md: 0 }}
    >
      <Box minW={0}>
        <HStack spacing={2} flexWrap="wrap">
          <Text
            fontWeight="800"
            fontSize="lg"
            color="#1A202C"
            noOfLines={1}
          >
            {student.full_name || "Estudiante sin nombre"}
          </Text>

          {showAlert && (
            <Tooltip
              label={`Nivel de alerta: ${student.alert.type}`}
              hasArrow
              placement="bottom"
            >
              <Tag
                size="sm"
                px={3}
                py={1}
                borderRadius="full"
                bg={`${student.alert.color}24`}
                color="#1A202C"
                border="1px solid"
                borderColor={`${student.alert.color}55`}
                fontWeight="800"
              >
                <HStack spacing={2}>
                  <Box
                    w="8px"
                    h="8px"
                    borderRadius="full"
                    bg={student.alert.color}
                    boxShadow={`0 0 0 4px ${student.alert.color}22`}
                  />
                  <Text>{student.alert.type}</Text>
                </HStack>
              </Tag>
            </Tooltip>
          )}
        </HStack>

        <Text color="#4A5568" fontSize="sm" mt={1}>
          CI: {student.ci || "—"} · Edad: {student.age_range || "—"} · Registro:{" "}
          {formatDate(student.register_date)}
        </Text>
      </Box>
    </Flex>
  );
}