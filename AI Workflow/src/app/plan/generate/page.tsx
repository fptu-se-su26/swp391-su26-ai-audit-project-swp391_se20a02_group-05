"use client";

import { useEffect, useState } from "react";
import { useSearchParams, useRouter } from "next/navigation";
import { motion } from "framer-motion";
import { Loader2, CheckCircle2, ArrowRight } from "lucide-react";
import { useTravelStore } from "@/store/useTravelStore";
import { Button } from "@/components/ui/button";
import { Card } from "@/components/ui/card";
import { Navbar } from "@/components/layout/Navbar";

export default function GeneratePlanPage() {
  const searchParams = useSearchParams();
  const router = useRouter();
  const { setPlan } = useTravelStore();
  
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [steps, setSteps] = useState([
    { text: "Analyzing your preferences...", done: false },
    { text: "Searching for flights and hotels...", done: false },
    { text: "Crafting day-by-day itinerary...", done: false },
    { text: "Optimizing budget...", done: false },
    { text: "Finalizing travel plan...", done: false },
  ]);

  useEffect(() => {
    let currentStep = 0;
    const interval = setInterval(() => {
      setSteps((prev) =>
        prev.map((s, i) => {
          if (i === currentStep) return { ...s, done: true };
          return s;
        })
      );
      currentStep++;
      if (currentStep >= steps.length) clearInterval(interval);
    }, 1500);

    const generatePlan = async () => {
      try {
        const dest = searchParams.get("destination");
        if (!dest) {
          router.push("/");
          return;
        }

        const reqBody = {
          destination: dest,
          budget: searchParams.get("budget") || "moderate",
          durationDays: parseInt(searchParams.get("days") || "3"),
          travelStyle: (searchParams.get("style") || "").split(","),
          travelers: 2,
          additionalNotes: searchParams.get("notes") || "",
        };

        const res = await fetch("/api/generate", {
          method: "POST",
          headers: { "Content-Type": "application/json" },
          body: JSON.stringify(reqBody),
        });

        const data = await res.json();
        
        if (data.success) {
          setPlan(data.plan);
          // Wait a bit to let animations finish
          setTimeout(() => {
            setLoading(false);
          }, 1000);
        } else {
          setError(data.error);
          setLoading(false);
        }
      } catch (e) {
        setError("Failed to connect to the AI server.");
        setLoading(false);
      }
    };

    generatePlan();

    return () => clearInterval(interval);
  }, [searchParams, router, setPlan]);

  if (error) {
    return (
      <div className="min-h-screen flex flex-col items-center justify-center">
        <h2 className="text-2xl text-destructive mb-4">Error: {error}</h2>
        <Button onClick={() => router.push("/")}>Go Back</Button>
      </div>
    );
  }

  if (loading) {
    return (
      <div className="min-h-screen flex flex-col bg-background">
        <Navbar />
        <main className="flex-1 flex items-center justify-center p-6">
          <Card className="max-w-md w-full p-8 space-y-6">
            <div className="flex items-center justify-center mb-8">
              <Loader2 className="w-12 h-12 text-primary animate-spin" />
            </div>
            <h2 className="text-xl font-semibold text-center">AI Agent is Working...</h2>
            <div className="space-y-4">
              {steps.map((step, i) => (
                <div key={i} className={`flex items-center space-x-3 transition-opacity duration-500 ${step.done ? 'opacity-100' : 'opacity-40'}`}>
                  {step.done ? (
                    <CheckCircle2 className="w-5 h-5 text-green-500" />
                  ) : (
                    <div className="w-5 h-5 rounded-full border-2 border-muted-foreground/30 border-t-primary animate-spin" />
                  )}
                  <span className="text-sm">{step.text}</span>
                </div>
              ))}
            </div>
          </Card>
        </main>
      </div>
    );
  }

  // Redirect to review page once loaded
  router.push("/plan/review");
  return null;
}
