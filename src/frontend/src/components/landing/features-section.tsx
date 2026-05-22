import { FeatureCard } from "./feature-card";

const FEATURES = [
  {
    title: "Double-Entry Ledger",
    description:
      "Every debit creates a matching credit. Full audit trail from transaction one — no reconciliation surprises.",
    icon: (
      <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="1.5">
        <path d="M3 6h18M3 12h18M3 18h18" strokeLinecap="round" />
      </svg>
    ),
  },
  {
    title: "Role-Based Accounts",
    description:
      "Customer and Admin roles with granular access control. Each account scoped to its owner by design.",
    icon: (
      <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="1.5">
        <circle cx="12" cy="8" r="4" />
        <path d="M4 20c0-4 3.6-7 8-7s8 3 8 7" strokeLinecap="round" />
      </svg>
    ),
  },
  {
    title: "Idempotent Transfers",
    description:
      "Duplicate TED requests are safely deduplicated via idempotency keys. Safe to retry, impossible to double-charge.",
    icon: (
      <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="1.5">
        <path d="M7 16V4m0 0L3 8m4-4 4 4M17 8v12m0 0 4-4m-4 4-4-4" strokeLinecap="round" strokeLinejoin="round" />
      </svg>
    ),
  },
  {
    title: "JWT Auth + Refresh Tokens",
    description:
      "Stateless authentication with automatic token rotation on every refresh. Revocation built in.",
    icon: (
      <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="1.5">
        <rect x="3" y="11" width="18" height="11" rx="2" />
        <path d="M7 11V7a5 5 0 0 1 10 0v4" strokeLinecap="round" />
      </svg>
    ),
  },
];

export function FeaturesSection() {
  return (
    <section id="features" className="px-6 py-24">
      <div className="mx-auto max-w-6xl">
        <div className="mb-12 text-center">
          <h2 className="font-display mb-4 text-3xl tracking-tight text-foreground sm:text-4xl">
            Built for production
          </h2>
          <p className="mx-auto max-w-xl text-muted">
            Everything you need to run a financial platform without the operational overhead.
          </p>
        </div>
        <div className="grid gap-6 sm:grid-cols-2 lg:grid-cols-4">
          {FEATURES.map((feature) => (
            <FeatureCard key={feature.title} {...feature} />
          ))}
        </div>
      </div>
    </section>
  );
}
