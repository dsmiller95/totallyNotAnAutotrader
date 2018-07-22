using TradingBot.Models;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace TradingBot.Pipes
{
    public class FakeMarketData : MarketDataSource
    {
        protected override async Task PollSource()
        {
            while (true)
            {
                await Task.Delay(5000);
                var update = new MarketUpdate(5, 9, 10, 4, DateTime.UtcNow);
                this.NotifyObservers(update);
            }
        }
    }
}
