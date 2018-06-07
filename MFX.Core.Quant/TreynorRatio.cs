namespace MFX.Core.Quant
{
    public class TreynorRatio
    {
        /// <summary>
        ///     Gets the treynor ratio for a performance time line compared to another.
        /// </summary>
        /// <param name="performancePerAnnum">The performance per annum.</param>
        /// <param name="riskFreeInterestRate">The risk free interest rate.</param>
        /// <param name="beta">The beta.</param>
        /// <returns></returns>
        public static double? GetTreynorRatio(double performancePerAnnum, double riskFreeInterestRate, double beta)
        {
            if (beta == 0) return null;

            return (performancePerAnnum - riskFreeInterestRate) / beta;
        }
    }
}