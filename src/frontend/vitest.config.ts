// =============================================================================
// vitest.config.ts
//
// Arquivo de configuração do Vitest — equivalente ao xUnit.runner.json no .NET.
//
// O QUE É O VITEST? Um test runner para JavaScript/TypeScript construído sobre
// o Vite. Entende TypeScript e JSX nativamente, ideal para projetos React.
// =============================================================================

import { defineConfig } from 'vitest/config'
import react from '@vitejs/plugin-react'
import path from 'path'

export default defineConfig({
  // ─────────────────────────────────────────────────────────────────────────
  // PLUGINS
  // O plugin React transforma arquivos .tsx/.jsx para JavaScript puro
  // antes de executar. Sem ele, um arquivo .tsx causaria erro de sintaxe.
  // ─────────────────────────────────────────────────────────────────────────
  plugins: [react()],

  // ─────────────────────────────────────────────────────────────────────────
  // RESOLVE: aliases de módulo
  //
  // O código usa `@/` como atalho para `./src/`. Por exemplo:
  //   import { cn } from '@/lib/utils'
  //
  // Esse alias está no tsconfig.json, mas o Vitest NÃO lê o tsconfig
  // automaticamente — temos que repetir aqui.
  // ─────────────────────────────────────────────────────────────────────────
  resolve: {
    alias: {
      '@': path.resolve(__dirname, './src'),
    },
  },

  test: {
    // ───────────────────────────────────────────────────────────────────────
    // ENVIRONMENT: jsdom
    //
    // Testes rodam no Node.js, que não tem APIs de browser (sem `document`,
    // sem `window`, sem `HTMLElement`). O jsdom é uma implementação JavaScript
    // do DOM do browser que roda dentro do Node.
    //
    // Sem isso, qualquer componente React lançaria "document is not defined".
    // ───────────────────────────────────────────────────────────────────────
    environment: 'jsdom',

    // ───────────────────────────────────────────────────────────────────────
    // SETUP FILES
    //
    // Roda ANTES de cada arquivo de teste.
    // Usamos para importar @testing-library/jest-dom, que adiciona matchers
    // como toBeInTheDocument(), toBeDisabled(), toHaveValue(), etc.
    // ───────────────────────────────────────────────────────────────────────
    setupFiles: ['./vitest.setup.ts'],

    // ───────────────────────────────────────────────────────────────────────
    // GLOBALS: true
    //
    // Torna describe, it, test, expect, beforeEach, afterEach disponíveis
    // globalmente, sem precisar importar em cada arquivo de teste.
    // ───────────────────────────────────────────────────────────────────────
    globals: true,
  },
})
