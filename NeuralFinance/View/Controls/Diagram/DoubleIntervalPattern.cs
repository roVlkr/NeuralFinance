using System;

namespace NeuralFinance.View.Controls.Diagram
{
    public class DoubleIntervalPattern : IntervalPattern<double>
    {
        // Range and offset of any interval
        private double intervalRange;

        // Range and offset of the axis (model)
        private double totalRange;
        private double totalOffset;

        public DoubleIntervalPattern(IntervalPatternMode mode,
            double minimum, double maximum, int minStops)
            : base(mode, minimum, maximum, minStops)
        { }

        public override double GetPercentage(double value)
        {
            return (value - totalOffset) / totalRange;
        }

        protected override bool InitializePattern(IntervalPatternMode mode,
            double minimum, double maximum, int minStopCount)
        {
            double potency = Math.Pow(10, Math.Floor(Math.Log10(maximum - minimum)));
            double[] potencyFactors = new[] { 0.2, 0.25, 0.5, 1, 1.5, 2, 2.5, 5, 10 };

            return GetRegularIntervalRange(potency, potencyFactors,
                mode, minimum, maximum, minStopCount,
                out intervalRange, out totalRange, out totalOffset,
                out minStop, out maxStop);
        }

        public override double Next(double value)
        {
            return value + intervalRange;
        }

        public override double Previous(double value)
        {
            return value - intervalRange;
        }
    }
}
