interface FeatureCardProps {
  icon: React.ReactNode;
  title: string;
  description: string;
}

export function FeatureCard({ icon, title, description }: FeatureCardProps) {
  return (
    <div className="group rounded-xl border border-border bg-surface p-6 transition-transform duration-300 hover:-translate-y-1">
      <div
        className="mb-4 flex h-10 w-10 items-center justify-center rounded-lg text-accent"
        style={{ backgroundColor: "var(--color-surface-raised, #1e1e24)" }}
      >
        {icon}
      </div>
      <h3 className="mb-2 text-base font-semibold text-foreground">{title}</h3>
      <p className="text-sm leading-relaxed text-muted">{description}</p>
    </div>
  );
}
