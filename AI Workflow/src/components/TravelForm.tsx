"use client";

import { useState } from "react";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { Sparkles, MapPin, CalendarDays, Users, DollarSign } from "lucide-react";
import { motion } from "framer-motion";

import { Button } from "@/components/ui/button";
import {
  Form,
  FormControl,
  FormDescription,
  FormField,
  FormItem,
  FormLabel,
  FormMessage,
} from "@/components/ui/form";
import { Input } from "@/components/ui/input";
import { Textarea } from "@/components/ui/textarea";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select";
import { Card, CardContent, CardHeader, CardTitle, CardDescription } from "@/components/ui/card";
import { travelPlanSchema, TravelPlanRequest } from "@/types";

const travelStyles = [
  "Adventure",
  "Relaxation",
  "Cultural",
  "Foodie",
  "Nightlife",
  "Nature",
  "Luxury",
];

export function TravelForm({ onSubmit }: { onSubmit: (data: TravelPlanRequest) => void }) {
  const [isSubmitting, setIsSubmitting] = useState(false);

  const form = useForm<TravelPlanRequest>({
    resolver: zodResolver(travelPlanSchema),
    defaultValues: {
      destination: "",
      budget: "moderate",
      travelStyle: [],
      travelers: 2,
      durationDays: 3,
      additionalNotes: "",
    },
  });

  const handleFormSubmit = async (data: TravelPlanRequest) => {
    setIsSubmitting(true);
    // Simulate slight delay for animation
    await new Promise((r) => setTimeout(r, 500));
    onSubmit(data);
    setIsSubmitting(false);
  };

  return (
    <Card className="w-full max-w-2xl mx-auto border-border/40 bg-card/40 backdrop-blur-md shadow-2xl">
      <CardHeader>
        <CardTitle className="text-3xl font-bold bg-gradient-to-br from-primary to-primary/60 bg-clip-text text-transparent">
          Design Your Dream Trip
        </CardTitle>
        <CardDescription>
          Tell our AI where you want to go and what you want to experience.
        </CardDescription>
      </CardHeader>
      <CardContent>
        <Form {...form}>
          <form onSubmit={form.handleSubmit(handleFormSubmit)} className="space-y-6">
            <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
              <FormField
                control={form.control}
                name="destination"
                render={({ field }) => (
                  <FormItem className="col-span-1 md:col-span-2">
                    <FormLabel className="flex items-center gap-2">
                      <MapPin className="w-4 h-4 text-primary" /> Destination
                    </FormLabel>
                    <FormControl>
                      <Input placeholder="e.g. Da Nang, Vietnam" {...field} className="bg-background/50" />
                    </FormControl>
                    <FormMessage />
                  </FormItem>
                )}
              />

              <FormField
                control={form.control}
                name="durationDays"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel className="flex items-center gap-2">
                      <CalendarDays className="w-4 h-4 text-primary" /> Duration (Days)
                    </FormLabel>
                    <FormControl>
                      <Input
                        type="number"
                        min={1}
                        {...field}
                        onChange={(e) => field.onChange(parseInt(e.target.value))}
                        className="bg-background/50"
                      />
                    </FormControl>
                    <FormMessage />
                  </FormItem>
                )}
              />

              <FormField
                control={form.control}
                name="travelers"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel className="flex items-center gap-2">
                      <Users className="w-4 h-4 text-primary" /> Travelers
                    </FormLabel>
                    <FormControl>
                      <Input
                        type="number"
                        min={1}
                        {...field}
                        onChange={(e) => field.onChange(parseInt(e.target.value))}
                        className="bg-background/50"
                      />
                    </FormControl>
                    <FormMessage />
                  </FormItem>
                )}
              />

              <FormField
                control={form.control}
                name="budget"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel className="flex items-center gap-2">
                      <DollarSign className="w-4 h-4 text-primary" /> Budget Level
                    </FormLabel>
                    <Select onValueChange={field.onChange} defaultValue={field.value}>
                      <FormControl>
                        <SelectTrigger className="bg-background/50">
                          <SelectValue placeholder="Select a budget" />
                        </SelectTrigger>
                      </FormControl>
                      <SelectContent>
                        <SelectItem value="budget">Budget-Friendly</SelectItem>
                        <SelectItem value="moderate">Moderate</SelectItem>
                        <SelectItem value="luxury">Luxury</SelectItem>
                        <SelectItem value="any">Any / Don't care</SelectItem>
                      </SelectContent>
                    </Select>
                    <FormMessage />
                  </FormItem>
                )}
              />
            </div>

            <FormField
              control={form.control}
              name="travelStyle"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>Travel Style (Select multiple)</FormLabel>
                  <div className="flex flex-wrap gap-2">
                    {travelStyles.map((style) => (
                      <div
                        key={style}
                        className={`cursor-pointer px-3 py-1.5 rounded-full text-sm font-medium transition-all ${
                          field.value.includes(style)
                            ? "bg-primary text-primary-foreground shadow-md"
                            : "bg-secondary text-secondary-foreground hover:bg-secondary/80"
                        }`}
                        onClick={() => {
                          const newValue = field.value.includes(style)
                            ? field.value.filter((s) => s !== style)
                            : [...field.value, style];
                          field.onChange(newValue);
                        }}
                      >
                        {style}
                      </div>
                    ))}
                  </div>
                  <FormMessage />
                </FormItem>
              )}
            />

            <FormField
              control={form.control}
              name="additionalNotes"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>Specific Requirements or Prompt</FormLabel>
                  <FormControl>
                    <Textarea
                      placeholder="e.g. Include beaches, local food, must have wheelchair accessibility..."
                      className="resize-none h-24 bg-background/50"
                      {...field}
                    />
                  </FormControl>
                  <FormDescription>
                    Provide any specific instructions for the AI planner.
                  </FormDescription>
                  <FormMessage />
                </FormItem>
              )}
            />

            <Button type="submit" className="w-full h-12 text-lg group" disabled={isSubmitting}>
              {isSubmitting ? (
                <motion.div
                  animate={{ rotate: 360 }}
                  transition={{ repeat: Infinity, duration: 1, ease: "linear" }}
                  className="mr-2"
                >
                  <Sparkles className="w-5 h-5" />
                </motion.div>
              ) : (
                <Sparkles className="w-5 h-5 mr-2 group-hover:text-amber-400 transition-colors" />
              )}
              {isSubmitting ? "Generating AI Plan..." : "Generate AI Travel Plan"}
            </Button>
          </form>
        </Form>
      </CardContent>
    </Card>
  );
}
