using System.Collections.Generic;

namespace MFX.Core.Quant
{
    public class Beta
    {
        /// <summary>
        ///     Gets the beta of a performance time line compared to another.
        /// </summary>
        /// <param name="performanceValues">The performance values.</param>
        /// <param name="comparePerformanceValues">The relation performance values.</param>
        /// <returns></returns>
        public double? GetBeta(IEnumerable<double> performanceValues, IEnumerable<double> comparePerformanceValues)
        {
            var varianceOfComparePerformance = StatisticFunctions.GetVariance(comparePerformanceValues);
            if (varianceOfComparePerformance == 0) return null;

            var covariance = StatisticFunctions.GetCoVariance(performanceValues, comparePerformanceValues);
            return covariance / varianceOfComparePerformance;
        }
    }
}