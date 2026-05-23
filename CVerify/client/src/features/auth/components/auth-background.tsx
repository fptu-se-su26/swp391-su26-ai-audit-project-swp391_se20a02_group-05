"use client";

import React from 'react';
import dynamic from 'next/dynamic';

// Dynamically import ShapeGrid with SSR disabled to prevent hydration/SSR mismatches
const ShapeGrid = dynamic(() => import('../../../components/reactbits/ShapeGrid'), {
  ssr: false,
  loading: () => <div className="absolute inset-0 bg-[#f5f5f5]" />
});

export const AuthBackground: React.FC = () => {
  return (
    <div className="absolute inset-0 overflow-hidden bg-[#f5f5f5]">
      <ShapeGrid
        speed={0.1}
        squareSize={40}
        direction="diagonal"
        borderColor="#e6e6e6ff"
        hoverFillColor="#f0f0f0ff"
        shape="square"
        hoverTrailAmount={5}
      />
    </div>
  );
};

export default AuthBackground;
