namespace MFX.Core.Quant
{
    public class NetSelectivity
    {
        /// <summary>
        ///     Gets the net selectivity for a performance time line compared to another.
        /// </summary>
        /// <param name="performancePerAnnum">The performance per annum.</param>
        /// <param name="comparePerformancePerAnnum">The compare performance per annum.</param>
        /// <param name="riskPerAnnum">The risk per annum.</param>
        /// <param name="compareRiskPerAnnum">The compare risk per annum.</param>
        /// <returns></returns>
        public static double? GetNetSelectivity(double performancePerAnnum, double comparePerformancePerAnnum,
            double riskPerAnnum, double compareRiskPerAnnum)
        {
            if (compareRiskPerAnnum == 0) return null;

            return performancePerAnnum - comparePerformancePerAnnum * riskPerAnnum / compareRiskPerAnnum;
        }
    }
}