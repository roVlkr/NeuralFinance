using System;
using System.Collections.Generic;
using System.Text;

namespace NeuralFinance.Model
{
    public class Estimate
    {
        public Estimate(double percentageIncrease, double absoluteValue)
        {
            PercentageIncrease = percentageIncrease;
            AbsoluteValue = absoluteValue;
            Positive = percentageIncrease >= 0;
        }

        public double PercentageIncrease { get; }

        public double AbsoluteValue { get; }

        public bool Positive { get; }

        public override string ToString()
        {
            return $"{AbsoluteValue:F4} ({(Positive ? "+" : "")}{PercentageIncrease*100:F2}%)";
        }
    }
}
