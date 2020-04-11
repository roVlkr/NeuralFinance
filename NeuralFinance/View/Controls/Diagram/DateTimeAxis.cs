using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace NeuralFinance.View.Controls.Diagram
{
    public class DateTimeAxis : Axis<DateTime>
    {
        static DateTimeAxis()
        {
            // Set minimum value to Now
            MinimumProperty.OverrideMetadata(typeof(DateTimeAxis), new PropertyMetadata(DateTime.Now,
                MinimumProperty.GetMetadata(typeof(Axis<DateTime>)).PropertyChangedCallback));

            // Set maximum value to Now + 1 second
            MaximumProperty.OverrideMetadata(typeof(DateTimeAxis), new PropertyMetadata(DateTime.Now.AddSeconds(1),
                MaximumProperty.GetMetadata(typeof(Axis<DateTime>)).PropertyChangedCallback));
        }

        protected override IntervalPattern<DateTime> CreateIntervalPattern()
        {
            return new DateTimeIntervalPattern(IntervalPatternMode, Minimum, Maximum, MinStops);
        }

        protected override string GetMarkerDescription(DateTime value)
        {
            var dateTimeBase = ((DateTimeIntervalPattern)IntervalPattern).DateTimeBase;

            if (dateTimeBase == DateTimeBase.Time)
            {
                return value.ToShortTimeString();
            }
            else if (dateTimeBase == DateTimeBase.Day)
            {
                return value.ToShortDateString();
            }
            else if (dateTimeBase == DateTimeBase.Month)
            {
                return value.ToString("MMMM y");
            }
            else  // Year
            {
                return value.ToString("yyyy");
            }
        }
    }
}
