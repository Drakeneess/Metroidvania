// src/theme/theme.js
import { extendTheme } from "@chakra-ui/styled-system";

const config = {
  initialColorMode: "dark",
  useSystemColorMode: false,
};

const colors = {
  brand: {
    50:"#f3e8ff",100:"#e9d5ff",200:"#d8b4fe",300:"#c084fc",
    400:"#a855f7",500:"#9333ea",600:"#7e22ce",700:"#6b21a8",
    800:"#581c87",900:"#3b0764",
  },
};

const semanticTokens = {
  colors: {
    "bg.canvas":     { default: "gray.50",        _dark: "gray.900" },
    "bg.card":       { default: "white",          _dark: "gray.800" },
    "bg.nav":        { default: "whiteAlpha.700", _dark: "blackAlpha.600" },
    "text.primary":  { default: "gray.800",       _dark: "whiteAlpha.900" },
    "text.muted":    { default: "gray.600",       _dark: "whiteAlpha.700" },
    "border.subtle": { default: "gray.200",       _dark: "whiteAlpha.300" },
  },
};

const styles = {
  global: {
    "html, body, #root": { height: "100%" },
    body: { bg: "bg.canvas", color: "text.primary" },
  },
};

const components = {
  Button: {
    defaultProps: { colorScheme: "brand", size: "sm", variant: "solid" },
    baseStyle: { borderRadius: "xl", fontWeight: "semibold" },
  },
  Input: {
    defaultProps: { variant: "outline", size: "sm" },
    variants: {
      outline: {
        field: {
          borderColor: "border.subtle",
          _focus: {
            borderColor: "brand.500",
            boxShadow: "0 0 0 1px var(--chakra-colors-brand-500)",
          },
          _placeholder: { color: "text.muted" },
        },
      },
    },
  },
  Heading: { baseStyle: { color: "text.primary" } },
  Text:    { baseStyle: { color: "text.primary" } },
};

const layerStyles = {
  card: {
    bg: "bg.card",
    borderWidth: "1px",
    borderColor: "border.subtle",
    borderRadius: "xl",
    p: 4,
  },
  stat: {
    bg: "bg.card",
    borderWidth: "1px",
    borderColor: "border.subtle",
    borderRadius: "xl",
    p: 4,
    textAlign: "left",
  },
};

const theme = extendTheme({
  config,
  colors,
  semanticTokens,
  styles,
  components,
  layerStyles,
  fonts: {
    heading:
      "Inter, system-ui, -apple-system, Segoe UI, Roboto, Ubuntu, Cantarell, 'Fira Sans', 'Droid Sans', 'Helvetica Neue', Arial, 'Apple Color Emoji', 'Segoe UI Emoji', 'Segoe UI Symbol'",
    body:
      "Inter, system-ui, -apple-system, Segoe UI, Roboto, Ubuntu, Cantarell, 'Fira Sans', 'Droid Sans', 'Helvetica Neue', Arial, 'Apple Color Emoji', 'Segoe UI Emoji', 'Segoe UI Symbol'",
  },
});

export default theme;
