const STEPS = [
  {
    number: "01",
    title: "Register",
    description:
      "Create your account with email and password. Receive JWT credentials instantly.",
  },
  {
    number: "02",
    title: "Create Account",
    description:
      "Open a ledger account. Your balance, limit, and transaction history tracked from creation.",
  },
  {
    number: "03",
    title: "Transfer Funds",
    description:
      "Send TED transfers with idempotency keys. Every transaction double-entry recorded.",
  },
];

export function HowItWorksSection() {
  return (
    <section id="how-it-works" className="px-6 py-24">
      <div className="mx-auto max-w-6xl">
        <div className="mb-12 text-center">
          <h2 className="font-display mb-4 text-3xl tracking-tight text-foreground sm:text-4xl">
            Up and running in minutes
          </h2>
          <p className="mx-auto max-w-xl text-muted">
            Three steps from zero to a fully operational financial account.
          </p>
        </div>

        <div className="relative grid gap-8 md:grid-cols-3">
          {/* Connector line (desktop) */}
          <div className="absolute left-0 right-0 top-8 hidden border-t border-border md:block" />

          {STEPS.map((step) => (
            <div key={step.number} className="relative text-center">
              <div
                className="mx-auto mb-4 flex h-16 w-16 items-center justify-center rounded-full border border-border bg-surface font-display text-2xl"
                style={{ color: "var(--color-accent)" }}
              >
                {step.number}
              </div>
              <h3 className="mb-2 text-lg font-semibold text-foreground">
                {step.title}
              </h3>
              <p className="text-sm leading-relaxed text-muted">
                {step.description}
              </p>
            </div>
          ))}
        </div>
      </div>
    </section>
  );
}
