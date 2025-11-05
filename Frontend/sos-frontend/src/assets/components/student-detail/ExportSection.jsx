// src/components/student-detail/ExportSection.jsx
import {
  Box,
  Button,
  HStack,
  VStack,
  Text,
  useToast,
  Divider,
} from "@chakra-ui/react";
import { FaFileCsv, FaFileExcel, FaFilePdf } from "react-icons/fa";
import { exportToCSV } from "../../../utils/export/exportToCSV";
import { exportToExcel } from "../../../utils/export/exportToExcel";
import { exportToPDF } from "../../../utils/export/exportToPDF";

export default function ExportSection({ payload, sessions = [], authUser }) {
  const toast = useToast();

  const handleExport = (fn, title, desc, successMsg, errorMsg) => {
    try {
      fn();
      toast({
        title,
        description: successMsg,
        status: "success",
        duration: 2500,
        isClosable: true,
      });
    } catch (e) {
      toast({
        title: `Error al exportar ${title}`,
        description: e?.message || errorMsg,
        status: "error",
        duration: 3000,
        isClosable: true,
      });
    }
  };

  return (
    <VStack
      align="stretch"
      spacing={8}
      p={6}
      bg="rgba(255,255,255,0.85)"
      borderRadius="16px"
      boxShadow="0 4px 16px rgba(0,0,0,0.08)"
      backdropFilter="blur(10px) saturate(160%)"
    >
      {/* Bloque 1 — Datos Crudos */}
      <Box>
        <Text
          fontWeight="700"
          fontSize="lg"
          mb={3}
          color="gray.700"
          textTransform="uppercase"
          letterSpacing="0.8px"
        >
          Datos crudos
        </Text>

        <HStack spacing={4} wrap="wrap">
          <Button
            onClick={() =>
              handleExport(
                () => exportToCSV({ payload, sessions }),
                "CSV",
                "Se descargó el archivo CSV.",
                "Archivo CSV generado correctamente.",
                "Intenta nuevamente."
              )
            }
            leftIcon={<FaFileCsv />}
            colorScheme="green"
            variant="solid"
            bg="green.500"
            _hover={{ bg: "green.600" }}
          >
            CSV
          </Button>

          <Button
            onClick={() =>
              handleExport(
                () => exportToExcel({ payload, sessions }),
                "Excel",
                "Se descargó el archivo .xlsx.",
                "Archivo Excel generado correctamente.",
                "Intenta nuevamente."
              )
            }
            leftIcon={<FaFileExcel />}
            colorScheme="blue"
            variant="outline"
            borderColor="blue.400"
            color="blue.600"
            _hover={{ bg: "blue.50" }}
          >
            Excel
          </Button>
        </HStack>
      </Box>

      <Divider borderColor="gray.200" />

      {/* Bloque 2 — Informe Profesional */}
      <Box>
        <Text
          fontWeight="700"
          fontSize="lg"
          mb={3}
          color="gray.700"
          textTransform="uppercase"
          letterSpacing="0.8px"
        >
          Informe profesional
        </Text>

        <Button
          onClick={() =>
            handleExport(
              () => exportToPDF({ payload, sessions, authUser }),
              "PDF clínico",
              "Se descargó el informe clínico.",
              "Informe PDF generado correctamente.",
              "Intenta nuevamente."
            )
          }
          leftIcon={<FaFilePdf />}
          bg="purple.700"
          color="white"
          _hover={{ bg: "purple.600" }}
          _active={{ bg: "purple.800" }}
          size="lg"
          width="100%"
        >
          Descargar PDF clínico
        </Button>
      </Box>
    </VStack>
  );
}
