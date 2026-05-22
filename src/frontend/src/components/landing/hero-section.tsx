import Link from "next/link";

export function HeroSection() {
  return (
    <section className="relative flex min-h-[calc(100vh-4rem)] items-center justify-center overflow-hidden px-6 py-24">
      {/* Radial glow */}
      <div
        className="pointer-events-none absolute inset-0"
        style={{
          background:
            "radial-gradient(ellipse 80% 50% at 50% 0%, rgba(79,142,247,0.08) 0%, transparent 70%)",
        }}
      />

      <div className="relative z-10 mx-auto max-w-4xl text-center">
        <div className="animate-fade-up mb-4 inline-flex items-center gap-2 rounded-full border border-border bg-surface px-4 py-1.5 text-xs text-muted">
          <span
            className="h-1.5 w-1.5 rounded-full bg-accent"
          />
          Institutional-grade infrastructure
        </div>

        <h1
          className="animate-fade-up animate-fade-up-delay-1 font-display mb-6 text-5xl leading-tight tracking-tight text-foreground sm:text-6xl lg:text-7xl"
        >
          The financial infrastructure
          <br />
          <span style={{ color: "var(--color-accent)" }}>your business</span>
          <br />
          deserves.
        </h1>

        <p
          className="animate-fade-up animate-fade-up-delay-2 mx-auto mb-10 max-w-2xl text-lg text-muted"
        >
          CoreLedger provides double-entry bookkeeping, role-based accounts,
          and real-time transaction ledgers — built for production from day one.
        </p>

        <div className="animate-fade-up animate-fade-up-delay-3 flex flex-col items-center gap-4 sm:flex-row sm:justify-center">
          <Link
            href="/signup"
            className="rounded-md bg-accent px-8 py-3 text-sm font-medium text-white transition-opacity hover:opacity-90"
          >
            Open an account
          </Link>
          <a
            href="#features"
            className="rounded-md border border-border px-8 py-3 text-sm font-medium text-muted transition-colors hover:border-foreground hover:text-foreground"
          >
            See features
          </a>
        </div>
      </div>
    </section>
  );
}
