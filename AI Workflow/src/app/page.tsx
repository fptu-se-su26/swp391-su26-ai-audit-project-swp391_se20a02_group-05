"use client";

import { motion } from "framer-motion";
import { Navbar } from "@/components/layout/Navbar";
import { TravelForm } from "@/components/TravelForm";
import { TravelPlanRequest } from "@/types";
import { useRouter } from "next/navigation";

export default function Home() {
  const router = useRouter();

  const handleGeneratePlan = (data: TravelPlanRequest) => {
    // In a real app, we would push to a generation page or show a modal
    // For now, let's encode the data to query params and navigate to /plan
    const searchParams = new URLSearchParams();
    searchParams.set("destination", data.destination);
    searchParams.set("days", data.durationDays.toString());
    searchParams.set("budget", data.budget);
    searchParams.set("style", data.travelStyle.join(","));
    if (data.additionalNotes) searchParams.set("notes", data.additionalNotes);

    router.push(`/plan/generate?${searchParams.toString()}`);
  };

  return (
    <div className="min-h-screen flex flex-col bg-background relative overflow-hidden">
      {/* Abstract Background */}
      <div className="absolute inset-0 z-0 overflow-hidden pointer-events-none">
        <div className="absolute -top-[30%] -left-[10%] w-[70%] h-[70%] rounded-full bg-primary/20 blur-[120px]" />
        <div className="absolute top-[20%] -right-[20%] w-[60%] h-[60%] rounded-full bg-blue-600/20 blur-[100px]" />
        <div className="absolute -bottom-[20%] left-[20%] w-[80%] h-[80%] rounded-full bg-purple-600/10 blur-[120px]" />
      </div>

      <Navbar />

      <main className="flex-1 flex flex-col items-center justify-center p-6 z-10">
        <motion.div
          initial={{ opacity: 0, y: 20 }}
          animate={{ opacity: 1, y: 0 }}
          transition={{ duration: 0.8 }}
          className="text-center max-w-3xl mb-12"
        >
          <div className="inline-block mb-4 px-3 py-1 rounded-full bg-primary/10 border border-primary/20 text-primary text-sm font-medium">
            ✨ Gemini 2.5 AI Powered
          </div>
          <h1 className="text-5xl md:text-7xl font-extrabold tracking-tight mb-6">
            Travel Planning,{" "}
            <span className="text-transparent bg-clip-text bg-gradient-to-r from-primary to-blue-500">
              Reimagined
            </span>
          </h1>
          <p className="text-lg md:text-xl text-muted-foreground">
            Experience the future of travel. Let our AI craft the perfect, highly-personalized itinerary for your next adventure in seconds.
          </p>
        </motion.div>

        <motion.div
          initial={{ opacity: 0, scale: 0.95 }}
          animate={{ opacity: 1, scale: 1 }}
          transition={{ duration: 0.8, delay: 0.2 }}
          className="w-full max-w-3xl"
        >
          <TravelForm onSubmit={handleGeneratePlan} />
        </motion.div>
      </main>
    </div>
  );
}
