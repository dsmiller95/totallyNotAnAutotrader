using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using TradingBot.Models.APIModels;

namespace TradingBot.Services
{
    public class BitMexApi
    {
        private string domain = "https://testnet.bitmex.com";
        private string apiKey = "3W4MvosJQ2zFMnOlhDstW-sS";
        private string apiSecret = "WeqIbt0PRKMHJ-WvGcSnOW6GccdkGcKI1sJ9J-0ynAAQzxuY";
        private int rateLimit;

        public BitMexApi(string bitmexKey = "", string bitmexSecret = "", string domain = "", int rateLimit = 5000)
        {
            this.apiKey = bitmexKey;
            this.apiSecret = bitmexSecret;
            this.rateLimit = rateLimit;
            this.domain = domain;
        }

        private string BuildQueryData(Dictionary<string, string> param)
        {
            if (param == null)
                return "";

            StringBuilder b = new StringBuilder();
            foreach (var item in param)
                b.Append(string.Format("&{0}={1}", item.Key, WebUtility.UrlEncode(item.Value)));

            try { return b.ToString().Substring(1); }
            catch (Exception) { return ""; }
        }

        private string BuildJSON(Dictionary<string, string> param)
        {
            if (param == null)
                return "";

            var entries = new List<string>();
            foreach (var item in param)
                entries.Add(string.Format("\"{0}\":\"{1}\"", item.Key, item.Value));

            return "{" + string.Join(",", entries) + "}";
        }

        public static string ByteArrayToString(byte[] ba)
        {
            StringBuilder hex = new StringBuilder(ba.Length * 2);
            foreach (byte b in ba)
                hex.AppendFormat("{0:x2}", b);
            return hex.ToString();
        }

        private long GetNonce()
        {
            DateTime yearBegin = new DateTime(1990, 1, 1);
            return DateTime.UtcNow.Ticks - yearBegin.Ticks;
        }

        private string Query(string method, string function, Dictionary<string, string> param = null, bool auth = false, bool json = false)
        {
            string paramData = json ? BuildJSON(param) : BuildQueryData(param);
            string url = "/api/v1" + function + ((method == "GET" && paramData != "") ? "?" + paramData : "");
            string postData = (method != "GET") ? paramData : "";

            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(domain + url);
            webRequest.Method = method;

            if (auth)
            {
                string nonce = GetNonce().ToString();
                string message = method + url + nonce + postData;
                byte[] signatureBytes = hmacsha256(Encoding.UTF8.GetBytes(apiSecret), Encoding.UTF8.GetBytes(message));
                string signatureString = ByteArrayToString(signatureBytes);

                webRequest.Headers.Add("api-nonce", nonce);
                webRequest.Headers.Add("api-key", apiKey);
                webRequest.Headers.Add("api-signature", signatureString);
            }

            try
            {
                if (postData != "")
                {
                    webRequest.ContentType = json ? "application/json" : "application/x-www-form-urlencoded";
                    var data = Encoding.UTF8.GetBytes(postData);
                    using (var stream = webRequest.GetRequestStream())
                    {
                        stream.Write(data, 0, data.Length);
                    }
                }

                using (WebResponse webResponse = webRequest.GetResponse())
                using (Stream str = webResponse.GetResponseStream())
                using (StreamReader sr = new StreamReader(str))
                {
                    return sr.ReadToEnd();
                }
            }
            catch (WebException wex)
            {
                using (HttpWebResponse response = (HttpWebResponse)wex.Response)
                {
                    if (response == null)
                        throw;

                    using (Stream str = response.GetResponseStream())
                    {
                        using (StreamReader sr = new StreamReader(str))
                        {
                            return sr.ReadToEnd();
                        }
                    }
                }
            }
        }

        //Gives us closest sell and closest buy that is on the orderbooks 
        public List<OrderBook> GetOrderBook(string symbol, int depth)
        {
            var param = new Dictionary<string, string>();
            param["symbol"] = symbol;
            param["depth"] = depth.ToString();
            string res = Query("GET", "/orderBook/l2", param);
            return JsonConvert.DeserializeObject<List<OrderBook>>(res);
        }

        //The defualt post order uses cross leverage unless otherwise specified
        public string PostOrderPostOnly(string symbol, string side, double price, int quantity)
        {
            var param = new Dictionary<string, string>();
            param["symbol"] = symbol;
            param["side"] = side;
            param["orderQty"] = quantity.ToString();
            param["orderType"] = "Limit";
            param["execInst"] = "ParticipateDoNotInitiate";
            param["displayQty"] = 0.ToString();
            param["price"] = price.ToString();
            return Query("POST", "/order", param, true);
        }

        public string CancelAllOpenOrders(string symbol, string note = "")
        {
            var param = new Dictionary<string, string>();
            param["symbol"] = symbol;
            param["text"] = note;
            return Query("DELETE", "/order/all", param, true, true);
        }

        public List<Instrument> GetActiveInstruments()
        {
            string res = Query("GET", "/instrument/active");
            return JsonConvert.DeserializeObject<List<Instrument>>(res);
        }

        public List<Instrument> GetInstrument(string symbol)
        {
            var param = new Dictionary<string, string>();
            param["symbol"] = symbol;
            string res = Query("GET", "/instrument", param);
            return JsonConvert.DeserializeObject<List<Instrument>>(res);
        }

        public List<Candle> GetCandleHistory(string symbol, int count, string size)
        {
            var param = new Dictionary<string, string>();
            param["symbol"] = symbol;
            param["count"] = count.ToString();
            param["reverse"] = true.ToString();
            param["partial"] = false.ToString();
            param["binSize"] = size;
            string res = Query("GET", "/trade/bucketed", param);
            return JsonConvert.DeserializeObject<List<Candle>>(res).OrderByDescending(a => a.TimeStamp).ToList();
        }

        public List<Position> GetOpenPositions(string symbol)
        {
            var param = new Dictionary<string, string>();
            string res = Query("GET", "/position", param, true);
            return JsonConvert.DeserializeObject<List<Position>>(res).Where(a => a.Symbol == symbol && a.IsOpen == true)
                .OrderByDescending(a => a.TimeStamp)
                .ToList();
        }

        public List<Order> GetOpenOrders(string symbol)
        {
            var param = new Dictionary<string, string>();
            param["symbol"] = symbol;
            param["reverse"] = true.ToString();
            string res = Query("GET", "/order", param, true);
            return JsonConvert.DeserializeObject<List<Order>>(res)
                .Where(a => a.OrderStatus == "New" || a.OrderStatus == "PartialyFilled")
                .OrderByDescending(a => a.TimeStamp).ToList();
        }

        public string EditOrderPrice(string orderId, double price)
        {
            var param = new Dictionary<string, string>();
            param["orderID"] = orderId;
            param["price"] = price.ToString();
            return Query("PUT", "/order", param, true, true);
        }

        public string GetOrders(string symbol)
        {
            var param = new Dictionary<string, string>();
            param["symbol"] = symbol;
            //param["filter"] = "{\"open\":true}";
            //param["columns"] = "";
            //param["count"] = 100.ToString();
            //param["start"] = 0.ToString();
            //param["reverse"] = false.ToString();
            //param["startTime"] = "";
            //param["endTime"] = "";
            return Query("GET", "/order", param, true);
        }

        public string MarketOrders(string symbol, string side, double price, int quantity)
        {
            var param = new Dictionary<string, string>();
            param["symbol"] = symbol;
            param["side"] = side;
            param["orderQty"] = quantity.ToString();
            param["orderType"] = "Market";
            param["execInst"] = "ParticipateDoNotInitiate";
            param["displayQty"] = 0.ToString();
            param["price"] = price.ToString();
            return Query("POST", "/order", param, true);
        }

        public string DeleteOrders()
        {
            var param = new Dictionary<string, string>();
            param["orderID"] = "de709f12-2f24-9a36-b047-ab0ff090f0bb";
            param["text"] = "cancel order by ID";
            return Query("DELETE", "/order", param, true, true);
        }

        private byte[] hmacsha256(byte[] keyByte, byte[] messageBytes)
        {
            using (var hash = new HMACSHA256(keyByte))
            {
                return hash.ComputeHash(messageBytes);
            }
        }

        #region RateLimiter

        private long lastTicks = 0;
        private object thisLock = new object();

        private void RateLimit()
        {
            lock (thisLock)
            {
                long elapsedTicks = DateTime.Now.Ticks - lastTicks;
                var timespan = new TimeSpan(elapsedTicks);
                if (timespan.TotalMilliseconds < rateLimit)
                    Thread.Sleep(rateLimit - (int)timespan.TotalMilliseconds);
                lastTicks = DateTime.Now.Ticks;
            }
        }

        #endregion RateLimiter
    }
}