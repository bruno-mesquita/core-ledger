// =============================================================================
// src/components/__tests__/example.test.tsx
//
// Exemplo de teste de componente React com @testing-library/react.
// Filosofia: "teste o que o usuário vê, não detalhes de implementação".
//
// ESTRUTURA DE CADA TESTE:
//   ARRANGE → prepara os dados de entrada
//   ACT     → renderiza o componente
//   ASSERT  → verifica que o DOM contém o esperado
// =============================================================================

import { render, screen } from '@testing-library/react'
import { FeatureCard } from '@/components/landing/feature-card'

// describe() agrupa testes relacionados. O nome aparece no output:
//   "FeatureCard > renders the title and description..."
describe('FeatureCard', () => {

  it('renders the title and description passed as props', () => {
    // ARRANGE: define os dados que o componente receberá
    const testTitle = 'Smart Budgeting'
    const testDescription = 'Track every transaction automatically.'
    const testIcon = <span data-testid="test-icon">$</span>

    // ACT: renderiza o componente no DOM simulado (jsdom)
    render(
      <FeatureCard
        icon={testIcon}
        title={testTitle}
        description={testDescription}
      />
    )

    // ASSERT: verifica que o texto aparece no DOM
    //
    // screen.getByText() procura um elemento pelo texto visível.
    // Se não encontrar, o teste falha e mostra o que havia no DOM.
    //
    // toBeInTheDocument() vem do @testing-library/jest-dom (vitest.setup.ts).
    expect(screen.getByText(testTitle)).toBeInTheDocument()
    expect(screen.getByText(testDescription)).toBeInTheDocument()
  })

  it('renders the icon node inside the card', () => {
    // getByTestId() encontra um elemento pelo atributo `data-testid`.
    // Útil quando não há texto visível ou role semântico para consultar.
    render(
      <FeatureCard
        icon={<span data-testid="my-icon">icon</span>}
        title="Any title"
        description="Any description"
      />
    )

    expect(screen.getByTestId('my-icon')).toBeInTheDocument()
  })
})
