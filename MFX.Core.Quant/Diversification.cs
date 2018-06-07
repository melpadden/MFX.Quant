namespace MFX.Core.Quant
{
    public class Diversification
    {
        /// <summary>
        ///     Gets the diversification for a performace time line compared to another.
        /// </summary>
        /// <param name="comparePerformancePerAnnum">The compare performance per annum.</param>
        /// <param name="riskPerAnnum">The risk per annum.</param>
        /// <param name="compareRiskPerAnnum">The compare risk per annum.</param>
        /// <param name="beta">The beta.</param>
        /// <param name="riskFreeRate">The rsik-free rate</param>
        /// <returns></returns>
        public static double? GetDiversification(double comparePerformancePerAnnum, double riskPerAnnum,
            double compareRiskPerAnnum, double beta, double riskFreeRate)
        {
            if (compareRiskPerAnnum == 0) return null;

            var firstTerm = riskPerAnnum / compareRiskPerAnnum - beta;
            var secondTerm = comparePerformancePerAnnum - riskFreeRate;
            return firstTerm * secondTerm;
        }
    }
}