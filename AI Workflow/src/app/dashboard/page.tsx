"use client";

import { useEffect, useMemo } from "react";
import { useRouter } from "next/navigation";
import { motion } from "framer-motion";
import { Download, Share2, Map, Calendar, DollarSign } from "lucide-react";
import { useTravelStore } from "@/store/useTravelStore";
import { Navbar } from "@/components/layout/Navbar";
import { BudgetChart } from "@/components/BudgetChart";
import { ItineraryTable } from "@/components/ItineraryTable";
import { Card, CardContent, CardHeader, CardTitle, CardDescription } from "@/components/ui/card";
import { Button } from "@/components/ui/button";

export default function DashboardPage() {
  const { currentPlan } = useTravelStore();
  const router = useRouter();

  useEffect(() => {
    if (!currentPlan) {
      router.push("/");
    }
  }, [currentPlan, router]);

  const flatActivities = useMemo(() => {
    if (!currentPlan) return [];
    const activities: any[] = [];
    currentPlan.days.forEach(day => {
      day.activities.forEach(act => {
        activities.push({
          day: day.day,
          time: act.time,
          activity: act.title,
          location: act.location,
          cost: act.cost,
          type: act.type,
        });
      });
    });
    return activities;
  }, [currentPlan]);

  if (!currentPlan) return null;

  return (
    <div className="min-h-screen bg-background flex flex-col">
      <Navbar />
      
      <main className="flex-1 container max-w-7xl py-8 px-4 md:px-6 space-y-8">
        {/* Header Section */}
        <div className="flex flex-col md:flex-row justify-between items-start md:items-end gap-4">
          <motion.div 
            initial={{ opacity: 0, x: -20 }}
            animate={{ opacity: 1, x: 0 }}
          >
            <h1 className="text-4xl font-extrabold tracking-tight mb-2">Trip Dashboard</h1>
            <p className="text-muted-foreground text-lg">
              {currentPlan.destination} • {currentPlan.days.length} Days
            </p>
          </motion.div>
          <div className="flex gap-2">
            <Button variant="outline"><Share2 className="w-4 h-4 mr-2"/> Share</Button>
            <Button><Download className="w-4 h-4 mr-2"/> Export PDF</Button>
          </div>
        </div>

        {/* Top Cards */}
        <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
          <Card className="bg-primary/5 border-primary/20">
            <CardContent className="p-6 flex items-center gap-4">
              <div className="p-4 bg-primary/10 rounded-full text-primary">
                <Map className="w-6 h-6" />
              </div>
              <div>
                <p className="text-sm font-medium text-muted-foreground">Destination</p>
                <p className="text-2xl font-bold">{currentPlan.destination}</p>
              </div>
            </CardContent>
          </Card>
          <Card className="bg-primary/5 border-primary/20">
            <CardContent className="p-6 flex items-center gap-4">
              <div className="p-4 bg-primary/10 rounded-full text-primary">
                <Calendar className="w-6 h-6" />
              </div>
              <div>
                <p className="text-sm font-medium text-muted-foreground">Duration</p>
                <p className="text-2xl font-bold">{currentPlan.days.length} Days</p>
              </div>
            </CardContent>
          </Card>
          <Card className="bg-primary/5 border-primary/20">
            <CardContent className="p-6 flex items-center gap-4">
              <div className="p-4 bg-primary/10 rounded-full text-primary">
                <DollarSign className="w-6 h-6" />
              </div>
              <div>
                <p className="text-sm font-medium text-muted-foreground">Total Est. Cost</p>
                <p className="text-2xl font-bold">\${currentPlan.estimatedCost}</p>
              </div>
            </CardContent>
          </Card>
        </div>

        <div className="grid grid-cols-1 lg:grid-cols-3 gap-8">
          {/* Charts Section */}
          <div className="lg:col-span-1 space-y-6">
            <Card>
              <CardHeader>
                <CardTitle>Budget Allocation</CardTitle>
                <CardDescription>Visual breakdown of your expenses</CardDescription>
              </CardHeader>
              <CardContent>
                <BudgetChart budgetSummary={currentPlan.budgetSummary} />
              </CardContent>
            </Card>

            <Card>
              <CardHeader>
                <CardTitle>Transportation</CardTitle>
              </CardHeader>
              <CardContent>
                <ul className="space-y-2 list-disc list-inside text-muted-foreground">
                  {currentPlan.transportation.map((t, i) => (
                    <li key={i}>{t}</li>
                  ))}
                </ul>
              </CardContent>
            </Card>
          </div>

          {/* Table Section */}
          <div className="lg:col-span-2 space-y-6">
            <Card className="h-full flex flex-col">
              <CardHeader>
                <CardTitle>Detailed Itinerary</CardTitle>
                <CardDescription>All your activities, times, and locations</CardDescription>
              </CardHeader>
              <CardContent className="flex-1">
                <ItineraryTable data={flatActivities} />
              </CardContent>
            </Card>
          </div>
        </div>
      </main>
    </div>
  );
}
