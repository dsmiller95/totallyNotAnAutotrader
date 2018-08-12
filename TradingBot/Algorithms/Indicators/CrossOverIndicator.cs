using System;
using System.Collections.Generic;
using TradingBot.Models;

namespace TradingBot.Algorithms.Indicators
{
    /// <summary>
    /// Interface used to represent the work that is done by a market analysis algorithm
    /// </summary>
    public class CrossOverIndicator : IIndicator
    {
        MovingAverageCalculator averageSmall;
        MovingAverageCalculator averageLarge;

        public CrossOverIndicator(int movingAvgSmall, int movingAvgLarge)
        {
            if(movingAvgSmall >= movingAvgLarge)
            {
                throw new ArgumentException("Small must be smaller than large");
            }
            averageSmall = new MovingAverageCalculator(movingAvgSmall);
            averageLarge = new MovingAverageCalculator(movingAvgLarge);
        }

        /// <summary>
        /// Analyze the list of market data, and return a number between -1 and 1 indicating favorability of the market
        /// </summary>
        /// <param name="marketBuffer"></param>
        /// <returns>-1 to recommend a buy, 0 for no recommendation, and 1 to recomment a sell.</returns>
        public float GetRecommendation(MarketUpdate currentValue, IEnumerable<MarketUpdate> marketBuffer)
        {
            var previousSmall = this.averageSmall.MovingAverage;
            var previousLarge = this.averageLarge.MovingAverage;

            var preDiff = previousSmall < previousLarge;

            this.averageSmall.CalculateMovingAverage(currentValue.Close);
            this.averageLarge.CalculateMovingAverage(currentValue.Close);

            var postDiff = this.averageSmall.MovingAverage < this.averageLarge.MovingAverage;

            if (preDiff && !postDiff)
            {
                // Buy! golden cross
                return -1;
            }
            if (!preDiff && postDiff)
            {
                // Sell! death cross
                return 1;
            }

            //TODO: try to figure out probability of cross based on slope

            return 0;
        }

        public string PrintRecommendation(float recomendation)
        {
            return $"CrossOver: {recomendation}";
        }
    }
}