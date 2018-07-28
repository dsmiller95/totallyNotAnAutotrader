using System;
using System.Collections.Generic;
using System.Linq;
using TradingBot.Models;
using TradingBot.Models.SimulatorModels;

namespace TradingBot.Pipes
{
    /// <summary>
    /// Naive implemntation of a market analyzer. Takes the average over the past 50 entries
    ///     sells if the current market is higher than average, buys if it's below
    /// </summary>
    public class FakeMarketAnalyzer : IMarketAnalyzer
    {
        public event EventHandler<SellRecommendation> Sell;
        public event EventHandler<BuyRecommendation> Buy;

        public MarketPosition Position
        {
            private get;
            set;
        }

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
            updates.AddFirst(value);
            if(updates.Count > maxDataLength)
            {
                updates.RemoveLast();
            }

            var average = updates.Average(update => update.Open);

            if(value.High < average)
            {
                Buy(this, new BuyRecommendation { BuyAmount = this.Position.USD * 0.1 / value.High });
            }
            if (value.Low > average)
            {
                Sell(this, new SellRecommendation { SellAmount = this.Position.BTC * 0.1 });
            }
        }
    }
}
