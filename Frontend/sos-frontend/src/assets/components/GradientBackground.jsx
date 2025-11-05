// src/assets/components/GradientBackground.jsx
import { Box, keyframes } from "@chakra-ui/react";

const gradientShift = keyframes`
  0% { background-position: 0% 50%; }
  50% { background-position: 100% 50%; }
  100% { background-position: 0% 50%; }
`;

export default function GradientBackground() {
  return (
    <Box
      position="fixed"
      inset="0"
      bg="linear-gradient(-45deg, #6B46C1, #1A202C, #000000, #232946)"
      backgroundSize="400% 400%"
      animation={`${gradientShift} 20s ease infinite`}
      zIndex={-1}
      opacity={0.45}
    />
  );
}
