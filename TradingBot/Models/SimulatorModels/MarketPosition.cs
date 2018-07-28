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

        public override string ToString()
        {
            return $"{USD:C2}\t{BTC:F5}BTC\t{orders.Count} orders";
        }
    }
}
