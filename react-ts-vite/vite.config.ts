import { defineConfig } from "vite";
import react from "@vitejs/plugin-react";
import tailwindcss from "@tailwindcss/vite";

// https://vite.dev/config/
export default defineConfig({
  plugins: [react(), tailwindcss()],
  server: {
    proxy: {
      "/api": {
        target: "http://localhost",
        changeOrigin: true,
        secure: false, // 自签证书需要这一行
      },
    },
  },
});

// it is used for all Vite commands, including both development (vite) and production builds (vite build).

// Your vite.config.ts file is the central configuration for your project.
// Vite just uses different parts of it depending on the command you run:

// npm run dev (vite):

// Uses the plugins (like react() and tailwindcss()) to transpile code on the fly and enable Hot Module Replacement (HMR).

// Uses the server section to configure the development server (e.g., to set up your /api proxy).

// npm run build (vite build):

// Also uses the plugins to correctly bundle, minify, and optimize your React and Tailwind code into static files for production.

// It ignores the server section because the build command doesn't run a server; it just creates files.
