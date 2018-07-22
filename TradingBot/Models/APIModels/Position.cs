using System;

namespace TradingBot.Models.APIModels
{
    public class Position
    {
        public DateTime TimeStamp { get; set; }
        public double? Leverage { get; set; }
        public double? CurrentCost { get; set; }
        public double? MarketPrice { get; set; }
        public double? MarketValue { get; set; }
        public double? BreakEvenPrice { get; set; }
        public double? LiquidationPrice { get; set; }
        public int? CurrentQty { get; set; }
        public bool IsOpen { get; set; }
        public string Symbol { get; set; }
    }
}