using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TradingBot.Models
{
    public class MarketUpdate : EventArgs
    {
        public MarketUpdate(int price, DateTime time)
        {
            this.CurrentPrice = price;
            this.Time = time;
        }

        public int CurrentPrice
        {
            private set;
            get;
        }
        public DateTime Time
        {
            private set;
            get;
        }
    }
}
