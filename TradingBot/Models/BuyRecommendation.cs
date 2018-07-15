using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TradingBot.Models
{
    public class BuyRecommendation : EventArgs
    {
        public double BuyAmount
        {
            get;
            set;
        }
    }
}
