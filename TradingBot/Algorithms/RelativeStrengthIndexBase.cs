namespace TradingBot.Algorithms
{
    public abstract class RelativeStrengthIndexBase<T>
    {
        protected T Gain;
        protected T Loss; 
        protected T AverageGain;
        protected T AverageLoss;
        protected T RelativeStrength;

        /// <summary>
        /// Current Relative Strength Index
        /// </summary>
        public double RSI { get; set; } = double.NaN;

        /// <summary>
        /// ToString
        /// </summary>
        /// <returns>String</returns>
        public override string ToString()
        {
            return string.Format("{0}", RSI);
        }
    }
}
