using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace NeuralFinance.View.Controls.Diagram
{
    public enum IntervalPatternMode
    {
        Outer,
        Inner,
        Adapted
    }

    public abstract class IntervalPattern<T> : IEnumerator<T> where T : IComparable
    {
        #region Fields
        /// <summary>
        /// The current value of this IEnumerator.
        /// </summary>
        protected T current;

        /// <summary>
        /// The minimal stop of this pattern.
        /// </summary>
        protected T minStop;

        /// <summary>
        /// The maximum stop of this pattern.
        /// </summary>
        protected T maxStop;
        #endregion

        #region Constructors
        public IntervalPattern(IntervalPatternMode mode, T minimum, T maximum, int minStopCount)
        {
            if (InitializePattern(mode, minimum, maximum, minStopCount))
            {
                Reset();
            }
            else
            {
                throw new Exception($"Initialization of IntervalPattern<{ typeof(T) }> failed!");
            }
        }
        #endregion

        #region Properties
        public T Current => current;
        object IEnumerator.Current => current;

        /// <summary>
        /// Returns the percentage of the current value with respect to the total
        /// range of this pattern.
        /// </summary>
        public double CurrentPercentage => GetPercentage(current);
        #endregion

        #region Methods
        /// <summary>
        /// Initializes the IntervalPattern with the given parameters. That is, it
        /// calculates the maxStop and minStop and all other members of the subclass.
        /// </summary>
        /// <param name="mode">
        /// The IntervalPatternMode. If it is Adapted, the pattern will be created
        /// with equally sized intervals from exactly minimum to maximum.
        /// If it is Inner, the resulting stops will be placed inside the range of
        /// maximum and minimum. If it is Outer, the resulting stops will be placed
        /// outside the range of maximum and minimum. 
        /// </param>
        /// <param name="minimum">
        /// The minimal value of the data to be presented.
        /// </param>
        /// <param name="maximum">
        /// The maximum value of the data to be presented.
        /// </param>
        /// <param name="minStopCount">
        /// The preferred minimum of stops of this pattern.
        /// </param>
        /// <returns>
        /// True, if and only if the initialization was successfull.
        /// </returns>
        protected abstract bool InitializePattern(IntervalPatternMode mode,
            T minimum, T maximum, int minStopCount);


        /// <summary>
        /// Calculates the next stop.
        /// Warning: <paramref name="value"/> has to be a stop itself!
        /// </summary>
        /// <param name="value">
        /// The value from which the next value is calculated.
        /// </param>
        /// <returns>
        /// The next stop.
        /// </returns>
        public abstract T Next(T value);


        /// <summary>
        /// Calculates the previous stop.
        /// Warning: <paramref name="value"/> has to be a stop itself!
        /// </summary>
        /// <param name="value">
        /// The value from which the previous value is calculated.
        /// </param>
        /// <returns>
        /// The previous stop.
        /// </returns>
        public abstract T Previous(T value);

        public bool MoveNext()
        {
            if (Next(current).CompareTo(maxStop) <= 0)
            {
                current = Next(current);
                return true;
            }

            return false;
        }

        public void Reset()
        {
            current = Previous(minStop);
        }

        public void Dispose()
        { }

        /// <summary>
        /// Calculates the percentage of <paramref name="value"/> with respect
        /// to the total range of this pattern.
        /// </summary>
        /// <param name="value">
        /// Value to calculate the percentage for.
        /// </param>
        /// <returns>
        /// The percentage of <paramref name="value"/> wit respect to the total
        /// range of this pattern.
        /// </returns>
        public abstract double GetPercentage(T value);
        #endregion

        #region Static Helper Function
        protected static bool GetRegularIntervalRange(double intervalBase, double[] factors,
            IntervalPatternMode mode, double minimum, double maximum, int minStopCount,
            out double intervalRange, out double totalRange, out double totalOffset,
            out double minStop, out double maxStop)
        {
            if (mode == IntervalPatternMode.Adapted)
            {
                intervalRange = (maximum - minimum) / minStopCount;
                totalRange = maximum - minimum;
                totalOffset = minimum;
                maxStop = maximum;
                minStop = minimum;

                return true;
            }

            var minIntervals = minStopCount + ((mode == IntervalPatternMode.Inner) ? 1 : -1);

            for (int i = factors.Length - 1; i >= 0; i--)
            {
                intervalRange = intervalBase * factors[i];
                int intervalCount;

                if (mode == IntervalPatternMode.Inner)
                {
                    // The lowest integral multiple of intervalRange that is higher than minimum
                    minStop = Math.Ceiling(minimum / intervalRange) * intervalRange;

                    // The highest count of intervals to not exceed the maximum from the Offset point
                    intervalCount = (int)Math.Floor((maximum - minStop) / intervalRange);

                    totalOffset = minimum;
                    totalRange = maximum - minimum;
                }
                else // if mode == IntervalPatternMode.Outer
                {
                    // The highest integral multiple of intervalRange that is lower than minimum
                    minStop = Math.Floor(minimum / intervalRange) * intervalRange;

                    // The lowest count of intervals to exceed the maximum from the Offset point
                    intervalCount = (int)Math.Ceiling((maximum - minStop) / intervalRange);

                    totalOffset = minStop;
                    totalRange = intervalCount * intervalRange;
                }

                if (intervalCount >= minIntervals)
                {
                    maxStop = minStop + intervalCount * intervalRange;
                    return true;
                }
            }

            // Standard initialization
            intervalRange = (maximum - minimum) / minStopCount;
            totalRange = maximum - minimum;
            totalOffset = minimum;
            maxStop = maximum;
            minStop = minimum;

            return false;
        }
        #endregion
    }
}
