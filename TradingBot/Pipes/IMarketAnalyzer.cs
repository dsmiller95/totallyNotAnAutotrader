﻿using TradingBot.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TradingBot.Models.SimulatorModels;

namespace TradingBot.Pipes
{
    public interface IMarketAnalyzer : IObserver<MarketUpdate>
    {
        event EventHandler<SellRecommendation> Sell;
        event EventHandler<BuyRecommendation> Buy;

        MarketPosition Position
        {
            set;
        }
    }
}
