using System;

namespace TradingBot.Models.SimulatorModels
{
    public class Order
    {
        public DateTime TimeStamp { get; set; }
        public string Symbol { get; set; }
        public OrderType OrderType { get; set; }
        public double? Price { get; set; }
        public double OrderQty { get; set; }
    }
}