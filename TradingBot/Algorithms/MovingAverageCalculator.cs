using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingBot.Models;

namespace TradingBot.Algorithms
{

    /// <summary>
    /// Calculates a moving average value over a specified window
    /// </summary>
    public sealed class MovingAverageCalculator : MovingAverageCalculatorBase<double>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="windowSize">Number of items until the calculator matures</param>
        public MovingAverageCalculator(int windowSize)
        {
            Reset(windowSize);
        }

        /// <summary>
        /// Updates the moving average with its next value.
        /// When IsMature is true and NextValue is called, a previous value will 'fall out' of the
        /// moving average.
        /// </summary>
        /// <param name="nextValue">The next value to be considered within the moving average.</param>
        public void CalculateMovingAverage(double nextValue)
        {
            // add new value to the sum
            Sum += nextValue;

            if (ValuesIn <  WindowSize)
            {
                // we haven't yet filled our window
                ValuesIn++;
            }
            else
            {
                // remove oldest value from sum
                Sum -= Values[NextValueIndex];
            }

            // store the value
            Values[NextValueIndex] = nextValue;

            // progress the next value index pointer
            NextValueIndex++;
            if (NextValueIndex == WindowSize)
            {
                NextValueIndex = 0;
            }

            MovingAverage = Sum / ValuesIn;
            Slope = MovingAverage - PreviousMovingAverage;
            PreviousMovingAverage = MovingAverage;

            // exponential moving average
            if (PreviousExponentialMovingAverage != double.MinValue)
            {
                ExponentialMovingAverage = ((nextValue - PreviousExponentialMovingAverage) * WeightingMultiplier) + PreviousExponentialMovingAverage;
                ExponentialSlope = ExponentialMovingAverage - PreviousExponentialMovingAverage;

                //update previous average
                PreviousExponentialMovingAverage = ExponentialMovingAverage;
            }
            else
            {
                ExponentialMovingAverage = nextValue;
                ExponentialSlope = 0.0;
                PreviousExponentialMovingAverage = ExponentialMovingAverage;
            }
        }

        /// <summary>
        /// Gets a value indicating whether enough values have been provided to fill the
        /// specified window size.  Values returned from NextValue may still be used prior
        /// to IsMature returning true, however such values are not subject to the intended
        /// smoothing effect of the moving average's window size.
        /// </summary>
        public bool Mature
        {
            get { return ValuesIn == WindowSize; }
        }

        /// <summary>
        /// Clears any accumulated state and resets the calculator to its initial configuration.
        /// Calling this method is the equivalent of creating a new instance.
        /// Must be called before first use
        /// </summary>
        public void Reset(int windowSize)
        {
            WindowSize = windowSize;
            Values = new double[WindowSize];
            WeightingMultiplier = 2.0 / (Values.Length + 1);
            NextValueIndex = 0;
            Sum = 0;
            ValuesIn = 0;
            PreviousExponentialMovingAverage = double.MinValue;
        }
    }
}
