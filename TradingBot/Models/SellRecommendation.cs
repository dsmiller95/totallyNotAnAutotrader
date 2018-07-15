using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TradingBot.Models
{
    public class SellRecommendation : EventArgs
    {
        public double SellAmount
        {
            get;
            set;
        }
    }
}
