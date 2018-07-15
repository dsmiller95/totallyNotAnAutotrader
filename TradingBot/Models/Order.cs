using System;

public class Order
{
    public DateTime TimeStamp { get; set; }
    public string Symbol { get; set; }
    public string OrderStatus { get; set; }
    public string OrderId { get; set; }
    public string OrderType { get; set; }
    public string Side { get; set; }
    public double? Price { get; set; }
    public int? OrderQty { get; set; }
    public int? DisplayQty { get; set; }
}