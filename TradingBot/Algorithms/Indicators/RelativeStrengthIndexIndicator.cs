using System;
using System.Collections.Generic;
using TradingBot.Models;

namespace TradingBot.Algorithms.Indicators
{
    /// <summary>
    /// Interface used to represent the work that is done by a market analysis algorithm
    /// </summary>
    public class RelativeStrengthIndexIndicator : IIndicator
    {
        RelativeStrengthIndex RSICalculator;

        public RelativeStrengthIndexIndicator(int period)
        {
            RSICalculator = new RelativeStrengthIndex(period);
        }

        /// <summary>
        /// Analyze the list of market data, and return a number between -1 and 1 indicating favorability of the market
        /// </summary>
        /// <param name="marketBuffer"></param>
        /// <returns>-1 to recommend a buy, 0 for no recommendation, and 1 to recomment a sell.</returns>
        public float GetRecommendation(MarketUpdate currentValue, IEnumerable<MarketUpdate> marketBuffer)
        {
            var lastRsi = RSICalculator.RSI;


            if (lastRsi == double.NaN || !RSICalculator.CalculateNextRSI(currentValue.Close))
            {
                return 0;
            }

            if(lastRsi < 30 && RSICalculator.RSI >= 30)
            {
                return -1;
            }

            if(lastRsi > 70 && RSICalculator.RSI <= 70)
            {
                return 1;
            }

            return 0;
        }

        public string PrintRecommendation(float recomendation)
        {
            return $"RSI: {recomendation}";
        }
    }
}