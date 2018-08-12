using System;
using System.Collections.Generic;
using System.Text;
using TradingBot.Models;

namespace TradingBot.Algorithms
{
    public abstract class MovingAverageCalculatorBase<T>
    {
        protected int WindowSize;
        protected int ValuesIn;
        protected int NextValueIndex;
        protected T[] Values;
        protected T Sum;

        protected T WeightingMultiplier;
        protected T PreviousMovingAverage;
        protected T PreviousExponentialMovingAverage;

        /// <summary>
        /// Current moving average
        /// </summary>
        public T MovingAverage { get; protected set; }

        /// <summary>
        /// Current slope
        /// </summary>
        public T Slope { get; protected set; }

        /// <summary>
        /// Current exponential moving average
        /// </summary>
        public T ExponentialMovingAverage { get; protected set; }

        /// <summary>
        /// Current exponential slope
        /// </summary>
        public T ExponentialSlope { get; protected set; }

        /// <summary>
        /// ToString
        /// </summary>
        /// <returns>String</returns>
        public override string ToString()
        {
            return string.Format("{0}:{1}, {2}:{3}", MovingAverage, Slope, ExponentialMovingAverage, ExponentialSlope);
        }
    }
}
