using TradingBot.Models;
using System;

namespace TradingBot.Services
{
    public interface IMarketSubject : IObservable<MarketUpdate>, IDisposable
    {
    }
}
