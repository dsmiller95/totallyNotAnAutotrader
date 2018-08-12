using System;
using System.Collections.Generic;
using TradingBot.Algorithms;
using TradingBot.Algorithms.Indicators;
using TradingBot.Pipes;

namespace ConsoleInterface
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            
            var historicalSource = new HistoricalDataSource("../../../../Market Data Files/coinbaseUSD_1-min_data_2014-12-01_to_2018-06-27.csv");

            var indicators = new List<IIndicator>
            {
                new CrossOverIndicator(50, 200),
                new RelativeStrengthIndexIndicator(20160)
            };

            var dataAnalyzer = new IndicatorBasedMarketAnalyzer(indicators);

            var marketSim = new MarketSimulatorMiddleware(historicalSource, dataAnalyzer);
            
            Console.Read();
        }
    }
}
