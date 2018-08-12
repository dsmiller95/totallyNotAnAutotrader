using CuttingEdge.Conditions;

namespace TradingBot.Algorithms
{
    public class RelativeStrengthIndex : RelativeStrengthIndexBase<double>
    {

        private int period;

        public RelativeStrengthIndex(int period)
        {
            this.period = period;
        }

        private double LastPrice = double.NaN;

        private double InitialGain;
        private double InitialLoss;

        private int TotalSamples;

        public bool CalculateNextRSI(double price)
        {
            try
            {
                if (LastPrice == double.NaN)
                {
                    return false;
                }

                var diff = price - LastPrice;

                // If we don't have all of our samples yet for the first value, we need to start queing them up
                if (TotalSamples <= period)
                {
                    if (diff >= 0)
                    {
                        Gain += diff;
                    }
                    else
                    {
                        Loss -= diff;
                    }
                    if(TotalSamples == period)
                    {
                        AverageGain = Gain / period;
                        AverageLoss = Loss / period;
                        RelativeStrength = Gain / Loss;

                        RSI = 100 - (100 / (1 + RelativeStrength));
                        return true;
                    }
                    return false;
                }

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

                RSI = 100 - (100 / (1 + RelativeStrength));
                return true;
            }
            finally
            {
                TotalSamples++;
                LastPrice = price;
            }
        }
    }
}








