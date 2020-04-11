using System;
using System.Collections.Generic;
using System.Text;

namespace NeuralFinance.View.Controls.Diagram
{
    public enum DateTimeBase
    {
        Year,
        Month,
        Day,
        Time
    }

    public class DateTimeIntervalPattern : IntervalPattern<DateTime>
    {
        private TimeSpan intervalRange;  // Just for regular patterns!
        private int multiple;            // Just for irregular patterns!

        private DateTime totalOffset;
        private TimeSpan totalRange;

        public DateTimeIntervalPattern(IntervalPatternMode mode,
            DateTime minimum, DateTime maximum, int minStops)
            : base(mode, minimum, maximum, minStops)
        { }

        public DateTimeBase DateTimeBase { get; private set; }

        public override double GetPercentage(DateTime value)
        {
            return (value - totalOffset).Ticks / (double)totalRange.Ticks; 
        }

        protected override bool InitializePattern(IntervalPatternMode mode,
            DateTime minimum, DateTime maximum, int minStopCount)
        {
            // A rough estimation for maximal interval
            var minIntervals = minStopCount + ((mode == IntervalPatternMode.Inner) ? 1 : -1);
            var prior = (maximum - minimum) / minIntervals;

            // Less than a month or adapted pattern => regular pattern
            if (prior < TimeSpan.FromDays(31) || mode == IntervalPatternMode.Adapted)
                return InitializeRegularPattern(prior, mode, minimum, maximum, minStopCount);
            else
                return InitializeIrregularPattern(prior, mode, minimum, maximum);
        }

        private bool InitializeRegularPattern(TimeSpan prior, IntervalPatternMode mode,
            DateTime minimum, DateTime maximum, int minStopCount)
        {
            TimeSpan intervalBase = TimeSpan.FromSeconds(1);
            double[] baseFactors = new double[] { 1, 5, 15, 30, 60 };
            DateTimeBase = DateTimeBase.Time;

            if (prior >= TimeSpan.FromDays(1))
            {
                intervalBase = TimeSpan.FromDays(1);
                baseFactors = new double[] { 1, 3, 7, 14, 21, 28 };
                DateTimeBase = DateTimeBase.Day;
            }
            else if (prior >= TimeSpan.FromHours(1))
            {
                intervalBase = TimeSpan.FromHours(1);
                baseFactors = new double[] { 1, 3, 6, 12, 15, 18, 21, 24 };
            }
            else if (prior >= TimeSpan.FromMinutes(1))
            {
                intervalBase = TimeSpan.FromMinutes(1);
            }

            bool success = GetRegularIntervalRange(intervalBase.Ticks, baseFactors,
                mode, minimum.Ticks,  maximum.Ticks, minStopCount,
                out double intervalRange, out double totalRange, out double totalOffset,
                out double minStop, out double maxStop);

            if (success)
            {
                this.intervalRange = new TimeSpan((long)intervalRange);
                this.totalRange = new TimeSpan((long)totalRange);
                this.totalOffset = new DateTime((long)totalOffset);
                this.minStop = new DateTime((long)minStop);
                this.maxStop = new DateTime((long)maxStop);

                return true;
            }

            return false;
        }

        private bool InitializeIrregularPattern(TimeSpan prior, IntervalPatternMode mode,
            DateTime minimum, DateTime maximum)
        {
            var year = TimeSpan.FromDays(366);
            var month = TimeSpan.FromDays(31);

            if (prior >= year)
            {
                DateTimeBase = DateTimeBase.Year;
                multiple = (int)Math.Floor(prior.Ticks / (double)year.Ticks);
            }
            else
            {
                DateTimeBase = DateTimeBase.Month;
                multiple = (int)Math.Floor(prior.Ticks / (double)month.Ticks);
            }

            if (mode == IntervalPatternMode.Inner)
            {
                if (DateTimeBase == DateTimeBase.Year)
                {
                    minStop = new DateTime(minimum.Year, 1, 1).AddYears(1);
                    maxStop = new DateTime(maximum.Year, 1, 1);
                }
                else
                {
                    minStop = new DateTime(minimum.Year, minimum.Month, 1).AddMonths(1);
                    maxStop = new DateTime(maximum.Year, maximum.Month, 1);
                }

                totalOffset = minimum;
                totalRange = maximum - minimum;
            }
            else if (mode == IntervalPatternMode.Outer)
            {
                if (DateTimeBase == DateTimeBase.Year)
                {
                    minStop = new DateTime(minimum.Year, 1, 1);
                    maxStop = new DateTime(maximum.Year, 1, 1).AddYears(1);
                }
                else
                {
                    minStop = new DateTime(minimum.Year, minimum.Month, 1);
                    maxStop = new DateTime(maximum.Year, maximum.Month, 1).AddMonths(1);
                }

                totalOffset = minStop;
                totalRange = maxStop - minStop;
            }

            return true;
        }

        public override DateTime Next(DateTime value)
        {
            // Regular patterns
            if (DateTimeBase == DateTimeBase.Time ||
                DateTimeBase == DateTimeBase.Day)
            {
                return value + intervalRange;
            }
            // Irregular patterns
            else if (DateTimeBase == DateTimeBase.Month)
            {
                return value.AddMonths(multiple);
            }
            else  // Year
            {
                return value.AddYears(multiple);
            }
        }

        public override DateTime Previous(DateTime value)
        {
            // Regular patterns
            if (DateTimeBase == DateTimeBase.Time ||
                DateTimeBase == DateTimeBase.Day)
            {
                return value - intervalRange;
            }
            // Irregular patterns
            else if (DateTimeBase == DateTimeBase.Month)
            {
                return value.AddMonths(-multiple);
            }
            else  // Year
            {
                return value.AddYears(-multiple);
            }
        }
    }
}
