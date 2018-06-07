using System.Collections.Generic;

namespace MFX.Core.Quant
{
    public class Correlation
    {
        /// <summary>
        ///     Gets the correlation between two performance time lines.
        /// </summary>
        /// <param name="performanceValues">The performance values.</param>
        /// <param name="comparePerformanceValues">The performance values to compare to.</param>
        /// <returns></returns>
        public static double? GetCorrelation(IEnumerable<double> performanceValues,
            IEnumerable<double> comparePerformanceValues)
        {
            return StatisticFunctions.GetCorrelation(performanceValues, comparePerformanceValues);
        }
    }
}