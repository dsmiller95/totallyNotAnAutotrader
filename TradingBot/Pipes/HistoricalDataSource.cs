using System;
using System.Collections.Generic;
using System.Text;
using TradingBot.Models;

namespace TradingBot.Pipes
{
    class HistoricalDataSource : IMarketSubject
    {
        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public IDisposable Subscribe(IObserver<MarketUpdate> observer)
        {
            throw new NotImplementedException();
        }
    }
}
