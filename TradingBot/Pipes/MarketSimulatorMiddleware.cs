using TradingBot.Models;
using System.Reactive.Linq;
using System.Collections.Generic;
using TradingBot.Models.SimulatorModels;
using System.Linq;
using System;

namespace TradingBot.Pipes
{
    /// <summary>
    /// A class used to encapsulate the interaction between the data source and the market analyzer
    ///     Automatically updates orders before new information is pushed to the analyzer
    ///     gives the analyzer access to data about the current position
    ///     
    /// </summary>
    public class MarketSimulatorMiddleware
    {
        private IMarketAnalyzer analyzer;

        private MarketPosition position;

        public MarketSimulatorMiddleware(IMarketSubject dataSource, IMarketAnalyzer dataAnalyzer)
        {
            this.analyzer = dataAnalyzer;
            this.position = new MarketPosition
            {
                orders = new List<Order>(),
                USD = 100,
                BTC = 1
            };

            dataSource
                .Select(this.UpdateModel)
                .Subscribe(dataAnalyzer);

            this.analyzer.Sell += new System.EventHandler<SellRecommendation>(this.OnSellOrder);
            this.analyzer.Buy += new System.EventHandler<BuyRecommendation>(this.OnBuyOrder);

            dataAnalyzer.Position = this.position;
        }

        private void OnSellOrder(object sender, SellRecommendation sell)
        {
            this.position.orders.Add(new Order
            {
                TimeStamp = sell.TimeStamp,
                Symbol = "BTC",
                OrderType = OrderType.MarketSell,
                OrderQty = sell.SellAmount
            });
        }

        private void OnBuyOrder(object sender, BuyRecommendation buy)
        {
            this.position.orders.Add(new Order
            {
                TimeStamp = buy.TimeStamp,
                Symbol = "BTC",
                OrderType = OrderType.MarketBuy,
                OrderQty = buy.BuyAmount
            });
        }

        /// <summary>
        /// updates the current position based on the new market price
        /// </summary>
        /// <param name="update"></param>
        /// <returns></returns>
        private MarketUpdate UpdateModel(MarketUpdate update)
        {
            this.position.orders = position.orders.Where(order =>
            {
                switch (order.OrderType)
                {
                    case OrderType.MarketBuy:
                        position.USD -= order.OrderQty * update.Open;
                        position.BTC += order.OrderQty;
                        Console.Out.WriteLineAsync($"{this.position}\t{this.position.BTC * update.Open + this.position.USD:C2}\tBought\t{order.OrderQty:F5} at {update.Open:F5}");
                        return false;
                    case OrderType.MarketSell:
                        position.USD += order.OrderQty * update.Open;
                        position.BTC -= order.OrderQty;
                        Console.Out.WriteLineAsync($"{this.position}\t{this.position.BTC * update.Open + this.position.USD:C2}\tSold\t{order.OrderQty:F5} at {update.Open:F5}");
                        return false;
                    case OrderType.LimitBuy:
                        if (DoesOrderExecute(order, update))
                        {
                            position.USD -= order.OrderQty * order.Price.Value;
                            position.BTC += order.OrderQty;
                            return false;
                        }
                        break;
                    case OrderType.LimitSell:
                        if (DoesOrderExecute(order, update))
                        {
                            position.USD += order.OrderQty * order.Price.Value;
                            position.BTC -= order.OrderQty;
                            return false;
                        }
                        break;
                }
                return true;
            }).ToList();

            return update;
        }

        private bool DoesOrderExecute(Order o, MarketUpdate u)
        {
            return (o.Price.HasValue &&
                o.Price.Value >= u.Low &&
                o.Price.Value <= u.High);
        }
    }
}
