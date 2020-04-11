using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace NeuralFinance.View.Controls.Diagram
{
    public class DoubleAxis : Axis<double>
    {
        static DoubleAxis()
        {
            // Set the default minimum value to 0.0
            MinimumProperty.OverrideMetadata(typeof(DoubleAxis), new PropertyMetadata(0.0,
                MinimumProperty.GetMetadata(typeof(Axis<double>)).PropertyChangedCallback));

            // Set the default maximum value to 1.0
            MaximumProperty.OverrideMetadata(typeof(DoubleAxis), new PropertyMetadata(1.0,
                MaximumProperty.GetMetadata(typeof(Axis<double>)).PropertyChangedCallback));
        }

        protected override IntervalPattern<double> CreateIntervalPattern()
        {
            return new DoubleIntervalPattern(IntervalPatternMode, Minimum, Maximum, MinStops);
        }

        protected override string GetMarkerDescription(double value)
        {
            return Math.Round(value, 15).ToString();
        }
    }
}
