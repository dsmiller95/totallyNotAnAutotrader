using FileHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace TradingBot.Models.FileFormats
{
    [DelimitedRecord(",")]
    class CSVHistoricalMarketData
    {
        public long Timestamp;

        public double Open;

        public double High;

        public double Low;

        public double Close;

        public double Volume_BTC;

        public double Volume_Currency;

        public double Weighted_Price;
    }
}
