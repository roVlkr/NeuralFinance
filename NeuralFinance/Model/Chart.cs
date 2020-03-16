using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NeuralFinance.Model
{
    public class Chart
    {
        private IDictionary<DateTime, double> data;

        public IDictionary<DateTime, double> Data
        {
            get { return data; }
            set
            {
                data = value;
                CalculateLogData();

                LogSampleMean = 0;
                foreach (var d in LogData.Values)
                {
                    LogSampleMean += d / data.Count;
                }

                LogSampleVariance = 0;
                foreach (var d in data.Values)
                {
                    LogSampleVariance += (d - LogSampleMean) * (d - LogSampleMean) / (data.Count - 1);
                }
            }
        }

        public SortedDictionary<DateTime, double> LogData { get; private set; }

        public double LogSampleMean { get; private set; }

        public double LogSampleVariance { get; private set; }

        private void CalculateLogData()
        {
            if (Data == null || Data.Count == 0)
            {
                LogData = null;
                return;
            }

            LogData = new SortedDictionary<DateTime, double>();
            double? a = null;
            foreach (var entry in Data)
            {
                if (a == null)
                {
                    a = entry.Value;
                    continue;
                }

                LogData.Add(entry.Key, LogIncrease(a.Value, entry.Value));
                a = entry.Value;
            }
        }

        public static double LogIncrease(double a, double b)
        {
            return Math.Log(b / a);
        }
    }
}
