// src/components/student-detail/ExportSection.jsx

import {
  Box,
  Button,
  HStack,
  VStack,
  Text,
  useToast,
  Divider,
  SimpleGrid,
  Icon,
} from "@chakra-ui/react";
import { FaFileCsv, FaFileExcel, FaFilePdf } from "react-icons/fa";

import { exportToCSV } from "../../../utils/export/exportToCSV";
import { exportToExcel } from "../../../utils/export/exportToExcel";
import { exportToPDF } from "../../../utils/export/exportToPDF";

function ExportCard({
  icon,
  title,
  description,
  buttonLabel,
  onClick,
  variant = "soft",
}) {
  const isPrimary = variant === "primary";

  return (
    <Box
      p={5}
      bg="rgba(255,255,255,0.76)"
      border="1px solid rgba(107,70,193,0.13)"
      borderRadius="18px"
      boxShadow="0 2px 12px rgba(15,12,41,0.07)"
      backdropFilter="blur(8px) saturate(150%)"
      transition="transform 0.2s ease, box-shadow 0.2s ease, border-color 0.2s ease"
      _hover={{
        transform: "translateY(-2px)",
        boxShadow: "0 8px 22px rgba(15,12,41,0.12)",
        borderColor: "rgba(107,70,193,0.24)",
      }}
    >
      <VStack align="stretch" spacing={4}>
        <HStack spacing={3} align="flex-start">
          <Box
            w="44px"
            h="44px"
            display="flex"
            alignItems="center"
            justifyContent="center"
            borderRadius="14px"
            bg={
              isPrimary
                ? "linear-gradient(135deg, #805AD5, #6B46C1)"
                : "rgba(107,70,193,0.10)"
            }
            color={isPrimary ? "white" : "#6B46C1"}
            boxShadow={isPrimary ? "0 6px 16px rgba(107,70,193,0.25)" : "none"}
            flexShrink={0}
          >
            <Icon as={icon} boxSize={5} />
          </Box>

          <Box>
            <Text color="#1A202C" fontWeight="900" fontSize="md">
              {title}
            </Text>

            <Text color="#718096" fontSize="sm" mt={1}>
              {description}
            </Text>
          </Box>
        </HStack>

        <Button
          onClick={onClick}
          width="100%"
          size={isPrimary ? "md" : "sm"}
          bg={
            isPrimary
              ? "linear-gradient(135deg, #805AD5, #6B46C1)"
              : "rgba(255,255,255,0.82)"
          }
          color={isPrimary ? "white" : "#6B46C1"}
          border="1px solid"
          borderColor={isPrimary ? "transparent" : "rgba(107,70,193,0.22)"}
          fontWeight="800"
          _hover={{
            bg: isPrimary
              ? "linear-gradient(135deg, #6B46C1, #553C9A)"
              : "rgba(107,70,193,0.08)",
            transform: "translateY(-1px)",
          }}
          _active={{
            transform: "translateY(0)",
          }}
        >
          {buttonLabel}
        </Button>
      </VStack>
    </Box>
  );
}

export default function ExportSection({ payload, sessions = [], authUser }) {
  const toast = useToast();

  const handleExport = (fn, title, successMsg, errorMsg) => {
    try {
      fn();

      toast({
        title,
        description: successMsg,
        status: "success",
        duration: 2500,
        isClosable: true,
      });
    } catch (error) {
      toast({
        title: `Error al exportar ${title}`,
        description: error?.message || errorMsg,
        status: "error",
        duration: 3000,
        isClosable: true,
      });
    }
  };

  return (
    <VStack
      align="stretch"
      spacing={6}
      p={{ base: 4, md: 5 }}
      bg="rgba(250,250,255,0.78)"
      border="1px solid rgba(107,70,193,0.12)"
      borderRadius="20px"
      boxShadow="0 4px 18px rgba(15,12,41,0.08)"
      backdropFilter="blur(8px) saturate(160%)"
    >
      <Box>
        <Text
          color="#1A202C"
          fontWeight="900"
          fontSize="md"
          letterSpacing="-0.01em"
        >
          Exportar información
        </Text>

        <Text color="#718096" fontSize="sm" mt={1}>
          Descarga los datos del estudiante, sus respuestas y sesiones
          asociadas para revisión externa o respaldo.
        </Text>
      </Box>

      <Box>
        <Text
          fontWeight="900"
          fontSize="xs"
          mb={3}
          color="#4A5568"
          textTransform="uppercase"
          letterSpacing="0.08em"
        >
          Datos crudos
        </Text>

        <SimpleGrid columns={{ base: 1, md: 2 }} spacing={4}>
          <ExportCard
            icon={FaFileCsv}
            title="Archivo CSV"
            description="Exporta datos tabulares simples para análisis rápido o integración básica."
            buttonLabel="Descargar CSV"
            onClick={() =>
              handleExport(
                () => exportToCSV({ payload, sessions }),
                "CSV",
                "Archivo CSV generado correctamente.",
                "Intenta nuevamente."
              )
            }
          />

          <ExportCard
            icon={FaFileExcel}
            title="Archivo Excel"
            description="Genera un archivo .xlsx para revisión estructurada en hojas de cálculo."
            buttonLabel="Descargar Excel"
            onClick={() =>
              handleExport(
                () => exportToExcel({ payload, sessions }),
                "Excel",
                "Archivo Excel generado correctamente.",
                "Intenta nuevamente."
              )
            }
          />
        </SimpleGrid>
      </Box>

      <Divider borderColor="rgba(107,70,193,0.14)" />

      <Box>
        <Text
          fontWeight="900"
          fontSize="xs"
          mb={3}
          color="#4A5568"
          textTransform="uppercase"
          letterSpacing="0.08em"
        >
          Informe profesional
        </Text>

        <ExportCard
          icon={FaFilePdf}
          title="PDF clínico"
          description="Genera un informe formal con datos del estudiante, resultados BDI, sesiones y usuario responsable."
          buttonLabel="Descargar PDF clínico"
          variant="primary"
          onClick={() =>
            handleExport(
              () => exportToPDF({ payload, sessions, authUser }),
              "PDF clínico",
              "Informe PDF generado correctamente.",
              "Intenta nuevamente."
            )
          }
        />
      </Box>
    </VStack>
  );
}