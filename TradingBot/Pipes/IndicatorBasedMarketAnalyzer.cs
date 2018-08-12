using System;
using System.Collections.Generic;
using System.Linq;
using TradingBot.Algorithms;
using TradingBot.Algorithms.Indicators;
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

        private const double confidenceModifier = 0.2;

        public MarketPosition Position
        {
            private get;
            set;
        }

        public IList<IIndicator> indicators;

        private LinkedList<MarketUpdate> updates = new LinkedList<MarketUpdate>();

        private const int maxDataLength = 50;
        private readonly TimeSpan maxDataSpan = TimeSpan.FromDays(1);

        public IndicatorBasedMarketAnalyzer(IList<IIndicator> indicators)
        {
            this.indicators = indicators;
        }

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
            while((value.Time - updates.Last.Value.Time) > maxDataSpan)
            {
                updates.RemoveLast();
            }

            var recommendations = indicators.Select(indicator => indicator.GetRecommendation(value, updates)).ToList();
            //var message = String.Join('\t', recommendations.Zip(indicators, (rec, ind) => ind.PrintRecommendation(rec)));
            //Console.Out.WriteLineAsync(message);

            var recommendation = recommendations.Average();
            if (recommendation <= -0.4)
            {
                // The closer to -1 it is, the higher percentage we should buy
                var recommendationStrength = ((-recommendation - 0.4) / 0.6) * confidenceModifier;
                Buy(this, new BuyRecommendation { BuyAmount = this.Position.USD * recommendationStrength / value.High });
            }else if(recommendation >= 0.4)
            {
                // The closer to 1 it is, the higher percentage we should sell
                var recommendationStrength = ((recommendation - 0.4) / 0.6) * confidenceModifier;
                Sell(this, new SellRecommendation { SellAmount = this.Position.BTC * recommendationStrength });
            }
        }
    }
}
