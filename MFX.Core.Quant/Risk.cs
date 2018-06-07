using System;
using System.Collections.Generic;
using System.Linq;

namespace MFX.Core.Quant
{
    public class Risk
    {
        /// <summary>
        ///     Gets the risk per annum of a performance time line.
        /// </summary>
        /// <param name="performanceValues">The performance values.</param>
        /// <param name="valuesPerYear">The values per year.</param>
        /// <returns></returns>
        public static double? GetRiskPerAnnum(IEnumerable<double> performanceValues, double valuesPerYear)
        {
            if (performanceValues == null) return null;
            if (performanceValues.Count() < Constants.MIN_PERFORMANCE_VALUES) return null;
            if (valuesPerYear < 0) return null;
            var standardDeviation = StatisticFunctions.GetStandardDeviation(performanceValues);
            var squareRootOfValuesPerYear = Math.Sqrt(valuesPerYear);
            return standardDeviation * squareRootOfValuesPerYear;
        }
    }
}