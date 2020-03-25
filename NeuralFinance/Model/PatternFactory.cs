using NeuralNetworks;
using VectorMath;
using System;
using System.Collections.Generic;
using System.Text;
using NeuralFinance.ViewModel;
using System.Linq;

namespace NeuralFinance.Model
{
    public class PatternFactory
    {
        public static List<TrainingPattern> CreateTrainingPatterns(Chart chart, IEnumerable<TimeRange> ranges, int input, int estimate)
        {
            double normalizeFunction(double x) => (x - chart.LogSampleMean) / Math.Sqrt(chart.LogSampleVariance);

            var disjointRanges = CreateDisjointRanges(ranges);
            var length = input + estimate;
            List<TrainingPattern> patterns = new List<TrainingPattern>();

            foreach (var range in disjointRanges)
            {
                var logDataSegment = chart.LogData
                                     .Where(entry => entry.Key >= range.StartTime && entry.Key <= range.StopTime)
                                     .Select(entry => entry.Value)
                                     .ToList();

                for (int i = 0; length + i <= logDataSegment.Count; i++)
                {
                    var inputRange = logDataSegment.GetRange(i, input);
                    var outputValue = logDataSegment.GetRange(i + input, estimate).Sum();

                    patterns.Add(new TrainingPattern(
                        new Vector(inputRange.ToArray()).Apply(normalizeFunction),
                        new Vector(new double[] { outputValue }).Apply(normalizeFunction)));
                }
            }            

            return patterns;
        }

        public static IEnumerable<TimeRange> CreateDisjointRanges(IEnumerable<TimeRange> ranges)
        {
            var ordered = ranges.OrderBy(range => range.StartTime);
            var joined = new List<TimeRange>();

            TimeRange first = null;
            foreach (var second in ordered)
            {
                if (first != null)
                {
                    // Intersection?
                    if (second.StartTime <= first.StopTime)
                    {
                        // Join both ranges
                        first.StopTime = Max(first.StopTime, second.StopTime);
                        continue;
                    }
                    else
                    {
                        joined.Add(first);
                    }
                }
                
                first = second;
            }

            if (first != null)
                joined.Add(first);

            return joined;
        }

        public static IEnumerable<TimeRange> CreateAlternateTimeRanges(IEnumerable<TimeRange> ranges, TimeRange total)
        {
            var disjoint = CreateDisjointRanges(ranges);  // already ordered
            var alternate = new List<TimeRange>();

            var startTime = total.StartTime;
            foreach (var range in disjoint)
            {
                if (range.StartTime <= startTime)
                {
                    startTime = range.StopTime + TimeSpan.FromMilliseconds(1);  // Must not be equal
                    continue;
                }

                var stopTime = range.StartTime - TimeSpan.FromMilliseconds(1);  // Must not be equal
                alternate.Add(new TimeRange(startTime, stopTime));

                startTime = stopTime;
            }

            if (total.StopTime > startTime)
            {
                alternate.Add(new TimeRange(startTime, total.StopTime));
            }

            return alternate;
        }

        public static DateTime Max(DateTime a, DateTime b)
        {
            return (a <= b) ? b : a;
        }
    }
}
