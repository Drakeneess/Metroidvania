// src/theme.js
import { extendTheme } from "@chakra-ui/react";

const theme = extendTheme({
  styles: {
    global: {
      "html, body": {
        bg: "radial-gradient(circle at top left, #1A1A2E, #0F0C29 60%, #000000)",
        color: "gray.100",
        fontFamily: "'Inter', 'Poppins', sans-serif",
        overflowX: "hidden",
      },
      a: {
        color: "purple.300",
        _hover: { color: "purple.100", textDecoration: "none" },
      },
    },
  },
  colors: {
    brand: {
      50: "#E9D8FD",
      100: "#D6BCFA",
      200: "#B794F4",
      300: "#9F7AEA",
      400: "#805AD5",
      500: "#6B46C1",
      600: "#553C9A",
      700: "#44337A",
      800: "#322659",
      900: "#21183C",
    },
    surface: "#232946",
    danger: "#EF476F",
    calm: "#00BBF9",
    success: "#06D6A0",
  },
  components: {
    Button: {
      baseStyle: {
        rounded: "xl",
        fontWeight: "bold",
      },
      variants: {
        solid: {
          bg: "brand.500",
          color: "white",
          _hover: { bg: "brand.400" },
        },
        ghost: {
          color: "gray.300",
          _hover: { bg: "whiteAlpha.200" },
        },
      },
    },
    Card: {
      baseStyle: {
        bg: "#232946",
        borderRadius: "xl",
        shadow: "xl",
        p: 6,
      },
    },
  },
});

export default theme;
