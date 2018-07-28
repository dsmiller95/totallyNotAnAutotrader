using System.Collections.Generic;
using TradingBot.Models;

namespace TradingBot.Algorithms
{
    /// <summary>
    /// Interface used to represent the work that is done by a market analysis algorithm
    /// </summary>
    public interface IIndicator
    {
        /// <summary>
        /// Analyze the list of market data, and return a number between -1 and 1 indicating favorability of the market
        /// </summary>
        /// <param name="marketBuffer"></param>
        /// <returns>-1 to recommend a buy, 0 for no recommendation, and 1 to recomment a sell.</returns>
        float GetRecommendation(IEnumerable<MarketUpdate> marketBuffer);
    }
}