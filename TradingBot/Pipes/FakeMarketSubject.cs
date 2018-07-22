using TradingBot.Models;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace TradingBot.Pipes
{
    public class FakeMarketSubject : MarketDataSource
    {
        protected override async Task PollSource()
        {
            while (true)
            {
                await Task.Delay(5000);
                var update = new MarketUpdate(8, DateTime.UtcNow);
                this.NotifyObservers(update);
            }
        }
    }
}
