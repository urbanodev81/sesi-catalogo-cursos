import vue from '@vitejs/plugin-vue'
import { defineConfig } from 'vitest/config'

// Em dev, o Vite repassa /api para a API .NET (dotnet run, porta 5070): mesma origem, sem CORS.
// No build, a saída vai para o wwwroot da API, que serve front e API num endereço só.
export default defineConfig({
  plugins: [vue()],
  server: {
    proxy: {
      '/api': 'http://localhost:5070',
      '/health': 'http://localhost:5070',
    },
  },
  build: {
    outDir: '../src/CatalogoCursos.Api/wwwroot',
    emptyOutDir: true,
  },
  // Testes do front (npm test): DOM simulado em memória, sem navegador.
  test: {
    environment: 'happy-dom',
    include: ['tests/**/*.test.ts'],
  },
})
