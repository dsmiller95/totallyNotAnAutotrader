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

            var dataSource = new FakeMarketData();
            var dataAnalyzer = new FakeMarketAnalyzer();

            historicalSource.Subscribe(dataAnalyzer);

            dataAnalyzer.Buy += OnBuySignal;
            dataAnalyzer.Sell += OnSellSignal;

            Console.Read();
        }

        private static void OnSellSignal(object sender, TradingBot.Models.SellRecommendation e)
        {
            Console.Out.WriteLineAsync($"SELL! {e.SellAmount} Units!");
        }

        private static void OnBuySignal(object sender, TradingBot.Models.BuyRecommendation e)
        {
            Console.Out.WriteLineAsync($"BUY! {e.BuyAmount} Units!");
        }
    }
}
