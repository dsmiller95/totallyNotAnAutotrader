using TradingBot.Models;
using System.Reactive.Linq;
using System.Collections.Generic;
using TradingBot.Models.SimulatorModels;
using System.Linq;

namespace TradingBot.Pipes
{
    class MarketSimulatorMiddleware
    {
        private IMarketAnalyzer analyzer;

        private MarketPosition position;

        public MarketSimulatorMiddleware(IMarketSubject dataSource, IMarketAnalyzer dataAnalyzer)
        {
            this.analyzer = dataAnalyzer;

            dataSource
                .Select(this.UpdateModel)
                .Subscribe(dataAnalyzer);

            this.analyzer.Sell += new System.EventHandler<SellRecommendation>(this.OnSellOrder);
            this.analyzer.Buy += new System.EventHandler<BuyRecommendation>(this.OnBuyOrder);

            this.position = new MarketPosition
            {
                orders = new List<Order>(),
                USD = 100,
                BTC = 1
            };

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

        private MarketUpdate UpdateModel(MarketUpdate update)
        {
            this.position.orders = position.orders.Where(order =>
            {
                switch (order.OrderType)
                {
                    case OrderType.MarketBuy:
                        position.USD -= order.OrderQty * update.Open;
                        position.BTC += order.OrderQty;
                        return false;
                    case OrderType.MarketSell:
                        position.USD += order.OrderQty * update.Open;
                        position.BTC -= order.OrderQty;
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
