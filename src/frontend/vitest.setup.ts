// =============================================================================
// vitest.setup.ts
//
// Executado uma vez antes de TODOS os arquivos de teste.
// Importa @testing-library/jest-dom, que adiciona matchers de DOM ao expect:
//
//   expect(element).toBeInTheDocument()    → elemento existe no DOM
//   expect(button).toBeDisabled()          → botão tem atributo disabled
//   expect(input).toHaveValue('hello')     → valor atual do input
//   expect(element).toBeVisible()          → elemento não está oculto
//   expect(element).toHaveTextContent('X') → texto do elemento contém 'X'
// =============================================================================

import '@testing-library/jest-dom'
