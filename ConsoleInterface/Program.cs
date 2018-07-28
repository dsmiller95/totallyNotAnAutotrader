using System;
using TradingBot.Pipes;

namespace ConsoleInterface
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            var historicalSource = new HistoricalDataSource("../../../../Market Data Files/coinbaseUSD_1-min_data_2014-12-01_to_2018-06-27.csv");
            
            var dataAnalyzer = new FakeMarketAnalyzer();

            var marketSim = new MarketSimulatorMiddleware(historicalSource, dataAnalyzer);
            
            Console.Read();
        }
    }
}
