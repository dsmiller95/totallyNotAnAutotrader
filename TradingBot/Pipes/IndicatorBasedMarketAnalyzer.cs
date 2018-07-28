using System;
using System.Collections.Generic;
using System.Linq;
using TradingBot.Algorithms;
using TradingBot.Models;
using TradingBot.Models.SimulatorModels;

namespace TradingBot.Pipes
{
    /// <summary>
    /// Market analyzer which relies on multiple indicators to make a buy or sell call
    /// </summary>
    public class IndicatorBasedMarketAnalyzer : IMarketAnalyzer
    {
        public event EventHandler<SellRecommendation> Sell;
        public event EventHandler<BuyRecommendation> Buy;

        public MarketPosition Position
        {
            private get;
            set;
        }

        public IList<IIndicator> indicators;

        private LinkedList<MarketUpdate> updates = new LinkedList<MarketUpdate>();

        private const int maxDataLength = 50;

        public void OnCompleted()
        {
            throw new NotImplementedException();
        }

        public void OnError(Exception error)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// This method gets called whenever new market data is pulled. Used to make buy/sell decisions
        ///     This is where we can aggregate some amount of the market data to do things like rolling averages
        /// </summary>
        /// <param name="value">The new market data</param>
        public void OnNext(MarketUpdate value)
        {
            //Control size of the buffer
            updates.AddFirst(value);
            if(updates.Count > maxDataLength)
            {
                updates.RemoveLast();
            }

            var recommendation = indicators.Select(indicator => indicator.GetRecommendation(updates)).Average();
            if (recommendation <= -0.8)
            {
                // The closer to -1 it is, the higher percentage we should buy
                var recommendationStrength = -recommendation - 0.8;
                Buy(this, new BuyRecommendation { BuyAmount = this.Position.USD * recommendationStrength / value.High });
            }else if(recommendation >= 0.8)
            {
                // The closer to 1 it is, the higher percentage we should sell
                var recommendationStrength = recommendation - 0.8;
                Sell(this, new SellRecommendation { SellAmount = this.Position.BTC * recommendationStrength });
            }
        }
    }
}
