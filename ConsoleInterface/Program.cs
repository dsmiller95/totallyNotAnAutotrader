using System;
using TradingBot.Pipes;

namespace ConsoleInterface
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            var dataSource = new FakeMarketSubject();
            var dataAnalyzer = new FakeMarketAnalyzer();

            dataSource.Subscribe(dataAnalyzer);

            dataAnalyzer.Buy += OnBuySignal;
            dataAnalyzer.Sell += OnSellSignal;

            Console.Read();
        }

        private static void OnSellSignal(object sender, TradingBot.Models.SellRecommendation e)
        {
            Console.Out.WriteLine($"SELL! {e.SellAmount} Units!");
        }

        private static void OnBuySignal(object sender, TradingBot.Models.BuyRecommendation e)
        {
            Console.Out.WriteLine($"BUY! {e.BuyAmount} Units!");
        }
    }
}
