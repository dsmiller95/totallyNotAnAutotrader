using CuttingEdge.Conditions;

namespace TradingBot.Algorithms
{
    public class RelativeStrengthIndex : RelativeStrengthIndexBase<double>
    {

        public void CaculateRelativeStrengthIndex(double[] price, int period)
        {
            Condition.Requires(price, "price")
                .IsNotEmpty();
            Condition.Requires(period, "period")
                .IsGreaterThan(0)
                .IsLessOrEqual(price.Length);

            RSI = new double[price.Length];

            Gain = 0.0;
            Loss = 0.0;

            RSI[0] = 0.0;

            for (var i = 1; i <= period; ++i)
            {
                var diff = price[i] - price[i - 1];
                if (diff >= 0)
                {
                    Gain += diff;
                }
                else
                {
                    Loss -= diff;
                }
            }

            AverageGain = Gain / period;
            AverageLoss = Loss / period;
            RelativeStrength = Gain / Loss;

            RSI[period] = 100 - (100 / (1 + RelativeStrength));

            for (var i = period + 1; i < price.Length; ++i)
            {
                var diff = price[i] - price[i - 1];

                if (diff >= 0)
                {
                    AverageGain = ((AverageGain * (period - 1)) + diff) / period;
                    AverageLoss = (AverageLoss * (period - 1)) / period;
                }
                else
                {
                    AverageLoss = ((AverageLoss * (period - 1)) - diff) / period;
                    AverageGain = (AverageGain * (period - 1)) / period;
                }

                RelativeStrength = AverageGain / AverageLoss;

                RSI[i] = 100 - (100 / (1 + RelativeStrength));
            }
        }
    }
}








