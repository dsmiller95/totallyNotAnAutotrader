using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TradingBot.Models
{
    public class MarketUpdate : EventArgs
    {
        public MarketUpdate(double open, double close, double high, double low, DateTime time)
        {
            this.Open = open;
            this.Close = close;
            this.High = high;
            this.Low = low;
            this.Time = time;
        }

        public double Open
        {
            private set;
            get;
        }

        public double Close
        {
            private set;
            get;
        }

        public double Low
        {
            private set;
            get;
        }

        public double High
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
