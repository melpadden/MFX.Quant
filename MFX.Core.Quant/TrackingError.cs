using System;
using System.Collections.Generic;
using System.Linq;

namespace MFX.Core.Quant
{
    public class TrackingError
    {
        /// <summary>
        ///     Gets the tracking error of a performance time line compared to another.
        /// </summary>
        /// <param name="performanceValues">The performance values.</param>
        /// <param name="comparePerformanceValues">The compare performance values.</param>
        /// <param name="valuesPerYear">The values per year.</param>
        /// <returns></returns>
        public static double? GetTrackingError(IEnumerable<double> performanceValues,
            IEnumerable<double> comparePerformanceValues, double valuesPerYear)
        {
            if (performanceValues == null || comparePerformanceValues == null || valuesPerYear < 0 ||
                !performanceValues.Any() || !comparePerformanceValues.Any())
                return null;

            var diffValues = new List<double>();
            var count = Math.Min(performanceValues.Count(), comparePerformanceValues.Count());
            for (var i = 0; i < count; i++)
                diffValues.Add(performanceValues.ElementAt(i) - comparePerformanceValues.ElementAt(i));

            var diffStandardDeviation = StatisticFunctions.GetStandardDeviation(diffValues);

            return diffStandardDeviation * Math.Sqrt(valuesPerYear);
        }

        public static double? CalculateTrackingError(
            IEnumerable<double> dailyPerformance1,
            IEnumerable<double> dailyPerformance2)
        {
            double? informationRatio;
            return CalculateTrackingError(dailyPerformance1, dailyPerformance2, out informationRatio);
        }

        public static double? CalculateTrackingError(
            IEnumerable<double> dailyPerformance1,
            IEnumerable<double> dailyPerformance2,
            out double? informationRatio)
        {
            informationRatio = null;
            var count = dailyPerformance1.Count();
            if (count < 1) return null;

            IList<double> differences = new List<double>();
            for (var i = 0; i < count; i++)
            {
                var value1 = 1 + dailyPerformance1.ElementAt(i);
                var value2 = 1 + dailyPerformance2.ElementAt(i);
                differences.Add((value1 <= 0 ? 0 : Math.Log(value1)) - (value2 <= 0 ? 0 : Math.Log(value2)));
            }

            var average = differences.Average();
            var sum = differences.Select(x => Math.Pow(x - average, 2)).Sum();
            var trackingError = Math.Pow(MathematicalFunctions.SafeDivision(sum, count - 1), 0.5) *
                                Math.Pow(260, 0.5);
            informationRatio = MathematicalFunctions.SafeDivision(average * 260, trackingError);
            return trackingError;
        }
    }
}