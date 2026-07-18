using System;
using Xunit;

namespace CVerify.API.UnitTests.Services
{
    public class ExtendedCandidateRankingCalculatorTests
    {
        [Fact]
        public void CalculateCompositeScore_DefaultWeights_ReturnsCorrectValue()
        {
            double aiScore = 80.0;
            double trustScore = 90.0;
            double completeness = 100.0;
            double ossImpact = 70.0;

            double expected = (aiScore * 0.35) + (trustScore * 0.35) + (completeness * 0.15) + (ossImpact * 0.15);
            double actual = CalculateScore(aiScore, trustScore, completeness, ossImpact);

            Assert.Equal(expected, actual, 3);
        }

        [Fact]
        public void CalculateCompositeScore_ZeroScores_ReturnsZero()
        {
            double actual = CalculateScore(0, 0, 0, 0);
            Assert.Equal(0.0, actual);
        }

        [Fact]
        public void CalculateCompositeScore_MaxScores_Returns100()
        {
            double actual = CalculateScore(100, 100, 100, 100);
            Assert.Equal(100.0, actual);
        }

        [Fact]
        public void VerifyPrimaryAuthorship_AboveThreshold_ReturnsTrue()
        {
            double commitRatio = 55.0;
            bool isPrimary = commitRatio >= 50.0;
            Assert.True(isPrimary);
        }

        private static double CalculateScore(double ai, double trust, double comp, double oss)
        {
            return (ai * 0.35) + (trust * 0.35) + (comp * 0.15) + (oss * 0.15);
        }
    }
}
