// src/assets/components/PageShell.jsx

import { Box, Container, Heading, Text, VStack } from "@chakra-ui/react";

export default function PageShell({ title, subtitle, children }) {
  return (
    <Container maxW="7xl" px={0}>
      <VStack align="stretch" spacing={6}>
        <Box>
          <Heading
            size="lg"
            color="white"
            letterSpacing="-0.03em"
          >
            {title}
          </Heading>

          {subtitle && (
            <Text mt={2} color="gray.400" fontSize="sm">
              {subtitle}
            </Text>
          )}
        </Box>

        {children}
      </VStack>
    </Container>
  );
}