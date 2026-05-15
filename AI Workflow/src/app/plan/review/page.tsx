"use client";

import { useEffect } from "react";
import { useRouter } from "next/navigation";
import { motion } from "framer-motion";
import { 
  MapPin, Calendar, DollarSign, Check, RefreshCw, Edit3, 
  Hotel, Utensils, Navigation
} from "lucide-react";
import { useTravelStore } from "@/store/useTravelStore";
import { Navbar } from "@/components/layout/Navbar";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Separator } from "@/components/ui/separator";
import { Badge } from "@/components/ui/badge";
import { ScrollArea } from "@/components/ui/scroll-area";
import { Tabs, TabsContent, TabsList, TabsTrigger } from "@/components/ui/tabs";

export default function ReviewPlanPage() {
  const { currentPlan } = useTravelStore();
  const router = useRouter();

  useEffect(() => {
    if (!currentPlan) {
      router.push("/");
    }
  }, [currentPlan, router]);

  if (!currentPlan) return null;

  const handleApprove = () => {
    router.push("/dashboard");
  };

  const handleRegenerate = () => {
    router.back();
  };

  return (
    <div className="min-h-screen bg-background flex flex-col">
      <Navbar />
      
      <main className="flex-1 container max-w-6xl py-8 px-4 md:px-6">
        <div className="flex flex-col md:flex-row justify-between items-start md:items-center mb-8 gap-4">
          <div>
            <h1 className="text-3xl font-bold tracking-tight">Your AI Travel Plan</h1>
            <p className="text-muted-foreground flex items-center gap-2 mt-2">
              <MapPin className="w-4 h-4" /> {currentPlan.destination}
            </p>
          </div>
          <div className="flex items-center gap-3 w-full md:w-auto">
            <Button variant="outline" onClick={handleRegenerate} className="flex-1 md:flex-none">
              <RefreshCw className="w-4 h-4 mr-2" /> Regenerate
            </Button>
            <Button onClick={handleApprove} className="flex-1 md:flex-none bg-green-600 hover:bg-green-700">
              <Check className="w-4 h-4 mr-2" /> Approve Plan
            </Button>
          </div>
        </div>

        <div className="grid grid-cols-1 lg:grid-cols-3 gap-8">
          {/* Main Itinerary */}
          <div className="lg:col-span-2 space-y-6">
            <Card>
              <CardHeader>
                <CardTitle>Trip Overview</CardTitle>
              </CardHeader>
              <CardContent>
                <p className="text-lg leading-relaxed">{currentPlan.summary}</p>
              </CardContent>
            </Card>

            <h2 className="text-2xl font-semibold mt-8 mb-4">Daily Itinerary</h2>
            <div className="space-y-8">
              {currentPlan.days.map((day, idx) => (
                <motion.div 
                  initial={{ opacity: 0, y: 20 }}
                  animate={{ opacity: 1, y: 0 }}
                  transition={{ delay: idx * 0.1 }}
                  key={idx}
                >
                  <Card className="overflow-hidden border-primary/20">
                    <div className="bg-primary/10 px-6 py-4 border-b border-primary/10 flex justify-between items-center">
                      <div>
                        <h3 className="font-bold text-xl">Day {day.day}: {day.title}</h3>
                        {day.date && <p className="text-sm text-muted-foreground">{day.date}</p>}
                      </div>
                    </div>
                    <CardContent className="p-0">
                      <div className="p-6">
                        <p className="text-muted-foreground mb-6">{day.summary}</p>
                        <div className="relative border-l border-muted-foreground/30 ml-4 space-y-8">
                          {day.activities.map((act, i) => (
                            <div key={i} className="relative pl-6">
                              <div className="absolute -left-[5px] top-1 w-2.5 h-2.5 rounded-full bg-primary" />
                              <div className="flex justify-between items-start mb-1">
                                <h4 className="font-semibold text-lg">{act.time} - {act.title}</h4>
                                <Badge variant="secondary">${act.cost}</Badge>
                              </div>
                              <p className="text-sm text-muted-foreground mb-2 flex items-center gap-1">
                                <MapPin className="w-3 h-3" /> {act.location}
                              </p>
                              <p className="text-sm">{act.description}</p>
                            </div>
                          ))}
                        </div>
                      </div>
                    </CardContent>
                  </Card>
                </motion.div>
              ))}
            </div>
          </div>

          {/* Sidebar Info */}
          <div className="space-y-6">
            <Card>
              <CardHeader>
                <CardTitle className="flex items-center gap-2">
                  <DollarSign className="w-5 h-5 text-green-500" /> Budget Breakdown
                </CardTitle>
              </CardHeader>
              <CardContent>
                <div className="text-3xl font-bold mb-6">${currentPlan.estimatedCost}</div>
                <div className="space-y-4">
                  {[
                    { label: "Accommodation", value: currentPlan.budgetSummary.accommodation, color: "bg-blue-500" },
                    { label: "Food & Dining", value: currentPlan.budgetSummary.food, color: "bg-orange-500" },
                    { label: "Activities", value: currentPlan.budgetSummary.activities, color: "bg-purple-500" },
                    { label: "Transportation", value: currentPlan.budgetSummary.transport, color: "bg-yellow-500" },
                  ].map((item, i) => (
                    <div key={i}>
                      <div className="flex justify-between text-sm mb-1">
                        <span>{item.label}</span>
                        <span className="font-medium">${item.value}</span>
                      </div>
                      <div className="h-2 bg-secondary rounded-full overflow-hidden">
                        <div 
                          className={`h-full ${item.color}`} 
                          style={{ width: `${(item.value / currentPlan.estimatedCost) * 100}%` }} 
                        />
                      </div>
                    </div>
                  ))}
                </div>
              </CardContent>
            </Card>

            <Tabs defaultValue="hotels">
              <TabsList className="w-full">
                <TabsTrigger value="hotels" className="flex-1"><Hotel className="w-4 h-4 mr-2"/> Hotels</TabsTrigger>
                <TabsTrigger value="food" className="flex-1"><Utensils className="w-4 h-4 mr-2"/> Food</TabsTrigger>
              </TabsList>
              <TabsContent value="hotels">
                <Card>
                  <CardContent className="p-4 space-y-4">
                    {currentPlan.hotels.map((hotel, i) => (
                      <div key={i} className="flex justify-between items-center pb-4 border-b last:border-0 last:pb-0">
                        <div>
                          <p className="font-medium">{hotel.name}</p>
                          <p className="text-sm text-muted-foreground flex items-center">
                            ★ {hotel.rating}
                          </p>
                        </div>
                        <Badge variant="outline">${hotel.pricePerNight}/n</Badge>
                      </div>
                    ))}
                  </CardContent>
                </Card>
              </TabsContent>
              <TabsContent value="food">
                <Card>
                  <CardContent className="p-4 space-y-4">
                    {currentPlan.foodRecommendations.map((food, i) => (
                      <div key={i} className="flex justify-between items-center pb-4 border-b last:border-0 last:pb-0">
                        <div>
                          <p className="font-medium">{food.name}</p>
                          <p className="text-sm text-muted-foreground">{food.type}</p>
                        </div>
                        <span className="text-green-500 font-medium">{food.priceRange}</span>
                      </div>
                    ))}
                  </CardContent>
                </Card>
              </TabsContent>
            </Tabs>
          </div>
        </div>
      </main>
    </div>
  );
}
