using System;

public class Candle
{
    public DateTime TimeStamp { get; set; }
    public double? Open { get; set; }
    public double? Close { get; set; }
    public double? High { get; set; }
    public double? Low { get; set; }
    public double? Volume { get; set; }
    public int Trades { get; set; }
    public int PreviousCandleCount { get; set; }
    public double? MovingAverage1 { get; set; }
    public double? MovingAverage2 { get; set; }
}