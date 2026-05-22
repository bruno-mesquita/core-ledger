import { NavBar } from "@/components/landing/nav-bar";
import { HeroSection } from "@/components/landing/hero-section";
import { FeaturesSection } from "@/components/landing/features-section";
import { HowItWorksSection } from "@/components/landing/how-it-works-section";
import { CtaFooterSection } from "@/components/landing/cta-footer-section";

export default function LandingPage() {
  return (
    <>
      <NavBar />
      <main>
        <HeroSection />
        <FeaturesSection />
        <HowItWorksSection />
      </main>
      <CtaFooterSection />
    </>
  );
}
