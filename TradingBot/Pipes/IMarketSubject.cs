using TradingBot.Models;
using System;

namespace TradingBot.Pipes
{
    public interface IMarketSubject : IObservable<MarketUpdate>, IDisposable
    {
    }
}
