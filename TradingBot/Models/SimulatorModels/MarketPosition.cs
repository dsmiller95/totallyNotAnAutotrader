using System;
using System.Collections.Generic;
using System.Text;

namespace TradingBot.Models.SimulatorModels
{
    public class MarketPosition
    {
        public IList<Order> orders;
        public double USD;
        public double BTC;
    }
}
