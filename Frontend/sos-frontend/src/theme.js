// src/theme.js

import { extendTheme } from "@chakra-ui/react";

const theme = extendTheme({
  config: {
    initialColorMode: "dark",
    useSystemColorMode: false,
  },

  fonts: {
    heading: "'Poppins', 'Inter', sans-serif",
    body: "'Inter', 'Poppins', sans-serif",
  },

  styles: {
    global: {
      "html, body": {
        bg: "#0F0C29",
        color: "gray.100",
        overflowX: "hidden",
      },

      body: {
        background:
          "radial-gradient(circle at top left, rgba(159, 122, 234, 0.32), transparent 34%), radial-gradient(circle at bottom right, rgba(0, 187, 249, 0.14), transparent 32%), linear-gradient(135deg, #1A1A2E 0%, #0F0C29 55%, #050510 100%)",
        backgroundAttachment: "fixed",
      },

      a: {
        color: "brand.200",
        transition: "0.2s ease",
        _hover: {
          color: "brand.100",
          textDecoration: "none",
        },
      },

      "::selection": {
        bg: "brand.500",
        color: "white",
      },
    },
  },

  colors: {
    brand: {
      50: "#F1E9FF",
      100: "#DCC7FF",
      200: "#C49BFF",
      300: "#A970FF",
      400: "#8B4DE8",
      500: "#6B35C8",
      600: "#5429A0",
      700: "#3D1F78",
      800: "#291452",
      900: "#170A33",
    },

    soul: {
      night: "#0F0C29",
      deep: "#1A1A2E",
      surface: "#232946",
      elevated: "#2B315A",
      hover: "#343B6B",
      border: "rgba(214, 188, 250, 0.18)",
      softBorder: "rgba(214, 188, 250, 0.10)",
      muted: "#A0AEC0",
    },

    danger: "#EF476F",
    calm: "#00BBF9",
    success: "#06D6A0",
    warning: "#FFD166",
  },

  radii: {
    xl: "18px",
    "2xl": "24px",
  },

  shadows: {
    soul: "0 18px 45px rgba(15, 12, 41, 0.45)",
    glow: "0 0 24px rgba(159, 122, 234, 0.35)",
    calmGlow: "0 0 24px rgba(0, 187, 249, 0.22)",
  },

  components: {
    Button: {
      baseStyle: {
        rounded: "xl",
        fontWeight: "700",
        letterSpacing: "0.02em",
        transition: "all 0.2s ease",
      },

      variants: {
        solid: {
          bg: "brand.500",
          color: "white",
          boxShadow: "glow",
          _hover: {
            bg: "brand.400",
            transform: "translateY(-1px)",
            boxShadow: "0 0 28px rgba(169, 112, 255, 0.45)",
          },
          _active: {
            bg: "brand.600",
            transform: "translateY(0)",
          },
        },

        ghost: {
          color: "gray.300",
          _hover: {
            bg: "whiteAlpha.200",
            color: "white",
          },
        },

        outline: {
          borderColor: "soul.border",
          color: "gray.200",
          _hover: {
            bg: "whiteAlpha.100",
            borderColor: "brand.300",
          },
        },

        danger: {
          bg: "danger",
          color: "white",
          _hover: {
            bg: "#FF5C83",
            transform: "translateY(-1px)",
          },
        },
      },
    },

    Input: {
      variants: {
        filled: {
          field: {
            bg: "rgba(35, 41, 70, 0.82)",
            border: "1px solid",
            borderColor: "soul.softBorder",
            color: "white",
            rounded: "xl",
            _placeholder: {
              color: "gray.500",
            },
            _hover: {
              bg: "rgba(43, 49, 90, 0.9)",
              borderColor: "brand.400",
            },
            _focus: {
              bg: "rgba(43, 49, 90, 0.95)",
              borderColor: "brand.300",
              boxShadow: "0 0 0 1px rgba(169, 112, 255, 0.8)",
            },
          },
        },
      },
      defaultProps: {
        variant: "filled",
      },
    },

    Textarea: {
      variants: {
        filled: {
          bg: "rgba(35, 41, 70, 0.82)",
          border: "1px solid",
          borderColor: "soul.softBorder",
          color: "white",
          rounded: "xl",
          _placeholder: {
            color: "gray.500",
          },
          _hover: {
            bg: "rgba(43, 49, 90, 0.9)",
            borderColor: "brand.400",
          },
          _focus: {
            bg: "rgba(43, 49, 90, 0.95)",
            borderColor: "brand.300",
            boxShadow: "0 0 0 1px rgba(169, 112, 255, 0.8)",
          },
        },
      },
      defaultProps: {
        variant: "filled",
      },
    },

    Select: {
      variants: {
        filled: {
          field: {
            h: "38px",
            minH: "38px",
            py: "0",
            px: "12px",
            fontSize: "sm",

            bg: "rgba(35, 41, 70, 0.88)",
            border: "1px solid",
            borderColor: "soul.softBorder",
            color: "gray.100",
            rounded: "xl",
            fontWeight: "500",
            cursor: "pointer",

            _hover: {
              bg: "rgba(43, 49, 90, 0.95)",
              borderColor: "brand.400",
            },

            _focus: {
              bg: "rgba(43, 49, 90, 0.98)",
              borderColor: "brand.300",
              boxShadow: "0 0 0 1px rgba(169, 112, 255, 0.8)",
            },

            _disabled: {
              opacity: 0.55,
              cursor: "not-allowed",
            },

            option: {
              backgroundColor: "#232946",
              color: "#F7FAFC",
            },
          },

          icon: {
            color: "brand.200",
          },
        },
      },

      sizes: {
        sm: {
          field: {
            h: "34px",
            minH: "34px",
            fontSize: "sm",
            px: "12px",
          },
          icon: {
            fontSize: "sm",
          },
        },

        md: {
          field: {
            h: "38px",
            minH: "38px",
            fontSize: "sm",
            px: "12px",
          },
        },
      },

      defaultProps: {
        variant: "filled",
        size: "md",
      },
    },

    Card: {
      baseStyle: {
        container: {
          bg: "rgba(35, 41, 70, 0.84)",
          border: "1px solid",
          borderColor: "soul.border",
          borderRadius: "2xl",
          boxShadow: "soul",
          backdropFilter: "blur(18px)",
        },
      },
    },

    Table: {
      variants: {
        simple: {
          th: {
            color: "gray.400",
            borderColor: "soul.border",
            fontSize: "xs",
            letterSpacing: "0.08em",
            textTransform: "uppercase",
          },
          td: {
            borderColor: "soul.softBorder",
            color: "gray.200",
          },
          tbody: {
            tr: {
              transition: "0.2s ease",
              _hover: {
                bg: "rgba(159, 122, 234, 0.10)",
              },
            },
          },
        },
      },
    },

    Modal: {
      baseStyle: {
        dialog: {
          bg: "soul.surface",
          border: "1px solid",
          borderColor: "soul.border",
          borderRadius: "2xl",
          boxShadow: "soul",
        },
        header: {
          color: "white",
        },
        body: {
          color: "gray.200",
        },
      },
    },
  },
});

export default theme;