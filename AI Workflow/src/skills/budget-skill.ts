export interface BudgetInput {
  totalBudget: number;
  durationDays: number;
  travelStyle: string[];
}

export interface BudgetOutput {
  accommodation: number;
  food: number;
  activities: number;
  transport: number;
  misc: number;
}

export const estimateBudget = (input: BudgetInput): BudgetOutput => {
  // Simple heuristic based on travel style and duration
  const { totalBudget, travelStyle } = input;
  
  let accomRatio = 0.4;
  let foodRatio = 0.25;
  let actRatio = 0.2;
  let transRatio = 0.1;
  let miscRatio = 0.05;

  if (travelStyle.includes("Luxury")) {
    accomRatio = 0.5;
    foodRatio = 0.3;
    actRatio = 0.1;
    transRatio = 0.05;
    miscRatio = 0.05;
  } else if (travelStyle.includes("Adventure")) {
    accomRatio = 0.2;
    foodRatio = 0.2;
    actRatio = 0.4;
    transRatio = 0.15;
    miscRatio = 0.05;
  }

  return {
    accommodation: Math.round(totalBudget * accomRatio),
    food: Math.round(totalBudget * foodRatio),
    activities: Math.round(totalBudget * actRatio),
    transport: Math.round(totalBudget * transRatio),
    misc: Math.round(totalBudget * miscRatio),
  };
};
