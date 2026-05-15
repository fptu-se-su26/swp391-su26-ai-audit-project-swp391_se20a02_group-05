"use client";

import { PieChart, Pie, Cell, ResponsiveContainer, Legend, Tooltip } from "recharts";

interface BudgetChartProps {
  budgetSummary: {
    accommodation: number;
    food: number;
    activities: number;
    transport: number;
    misc: number;
  };
}

const COLORS = ["#0088FE", "#00C49F", "#FFBB28", "#FF8042", "#A28DFF"];

export function BudgetChart({ budgetSummary }: BudgetChartProps) {
  const data = [
    { name: "Accommodation", value: budgetSummary.accommodation },
    { name: "Food & Dining", value: budgetSummary.food },
    { name: "Activities", value: budgetSummary.activities },
    { name: "Transport", value: budgetSummary.transport },
    { name: "Miscellaneous", value: budgetSummary.misc },
  ];

  return (
    <div className="h-[300px] w-full">
      <ResponsiveContainer width="100%" height="100%">
        <PieChart>
          <Pie
            data={data}
            cx="50%"
            cy="50%"
            innerRadius={60}
            outerRadius={100}
            fill="#8884d8"
            paddingAngle={5}
            dataKey="value"
          >
            {data.map((entry, index) => (
              <Cell key={`cell-${index}`} fill={COLORS[index % COLORS.length]} />
            ))}
          </Pie>
          <Tooltip 
            formatter={(value: any) => [`$${value}`, "Budget"]}
            contentStyle={{ borderRadius: "8px", border: "none", boxShadow: "0 4px 6px -1px rgb(0 0 0 / 0.1)" }}
          />
          <Legend />
        </PieChart>
      </ResponsiveContainer>
    </div>
  );
}
