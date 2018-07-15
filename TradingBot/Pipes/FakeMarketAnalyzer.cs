using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TradingBot.Models;

namespace TradingBot.Pipes
{
    public class FakeMarketAnalyzer : IMarketAnalyzer
    {
        public event EventHandler<SellRecommendation> Sell;
        public event EventHandler<BuyRecommendation> Buy;

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

        public void OnNext(MarketUpdate value)
        {
            updates.AddFirst(value);
            if(updates.Count > maxDataLength)
            {
                updates.RemoveLast();
            }

            var pricePoint = updates.Average(update => update.CurrentPrice) - 10;
            if (pricePoint > 0)
            {
                Sell(this, new SellRecommendation { SellAmount = pricePoint });
            }
            else
            {
                Buy(this, new BuyRecommendation { BuyAmount = -pricePoint });
            }
        }
    }
}
