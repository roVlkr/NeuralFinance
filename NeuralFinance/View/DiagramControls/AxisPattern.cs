using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuralFinance.View.DiagramControls
{
    struct AxisPattern
    {
        public AxisPattern(double minValue, double maxValue, int maxStops)
        {
            double[] markers = new double[] { 2.5, 2, 1, 0.5, 0.25, 0.2, 0.1 };

            double range = maxValue - minValue;
            double lowerPotency = Math.Pow(10, (int)Math.Log10(range));

            double step = markers[0] * lowerPotency;
            for (int i = 1; i < markers.Length; i++)
            {
                if (range / (markers[i] * lowerPotency) > maxStops)
                    break;
                else
                    step = markers[i] * lowerPotency;
            }

            Step = step;
            MinStop = Math.Floor(minValue / step) * step;
            MaxStop = Math.Ceiling(maxValue / step) * step;
        }

        public double Step { get; set; }

        public double MinStop { get; set; }

        public double MaxStop { get; set; }

        public int StopCount => (int) ((MaxStop - MinStop) / Step);

        public bool IsValid => Step > 0 && MinStop < MaxStop;
    }
}
