namespace MFX.Core.Quant
{
    public class SharpeRatio
    {
        /// <summary>
        ///     Gets the sharpe ratio.
        /// </summary>
        /// <param name="performancePerAnnum">The performance per annum.</param>
        /// <param name="riskFreeInterestRate">The risk free interest rate.</param>
        /// <param name="riskPerAnnum">The risk per annum.</param>
        /// <returns></returns>
        public static double? GetSharpeRatio(double performancePerAnnum, double riskFreeInterestRate,
            double riskPerAnnum)
        {
            if (riskPerAnnum == 0) return null;
            return (performancePerAnnum - riskFreeInterestRate) / riskPerAnnum;
        }
    }
}