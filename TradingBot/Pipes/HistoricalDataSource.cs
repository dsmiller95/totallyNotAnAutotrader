using FileHelpers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TradingBot.Models;
using TradingBot.Models.FileFormats;

namespace TradingBot.Pipes
{
    public class HistoricalDataSource : MarketDataSource
    {
        private CSVHistoricalMarketData[] fileData;

        public HistoricalDataSource(string filePath)
        {
            var engine = new FileHelperEngine<CSVHistoricalMarketData>();
            this.fileData = engine.ReadFile(filePath);

            this.BeginPoll();
        }

        protected override async Task PollSource()
        {
            foreach(var data in fileData)
            {
                await Task.Yield();
                var update = new MarketUpdate(data.Open, data.Close, data.High, data.Low, UnixTimeToDateTime(data.Timestamp));
                this.NotifyObservers(update);
            }
        }

        private DateTime UnixTimeToDateTime(long unixTime)
        {
            // Unix timestamp is seconds past epoch
            DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixTime).ToLocalTime();
            return dtDateTime;
        }
    }
}
