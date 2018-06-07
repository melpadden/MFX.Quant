using System;
using System.Collections.Generic;
using System.Linq;

namespace MFX.Core.Quant
{
    public static class StatisticFunctions
    {
        #region Methods

        /// <summary>
        ///     Percentiles the disc.
        /// </summary>
        /// <param name="enumerable">The enumerable.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static double GetPercentileDisc(IEnumerable<double?> enumerable, double value)
        {
            return GetPercentileDisc(enumerable.Where(v => v.HasValue).Select(v => v.Value), value);
        }

        /// <summary>
        ///     Percentiles the disc.
        /// </summary>
        /// <param name="enumerable">The enumerable.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static double GetPercentileDisc(IEnumerable<double> enumerable, double value)
        {
            var count = enumerable.Count();
            if (count == 0) return 0;
            if (count == 1) return enumerable.First();
            var data = enumerable.OrderBy(v => v).ToList();
            var idx = Convert.ToInt32(Math.Ceiling(value * data.Count));
            return data[idx];
        }

        public static double GetMedian(IEnumerable<double?> enumerable)
        {
            return GetMedian(enumerable.Where(v => v.HasValue).Select(v => v.Value));
        }

        public static double GetMedian(IEnumerable<double> enumerable)
        {
            var count = enumerable.Count();
            if (count == 0) return 0;
            if (count == 1) return enumerable.First();
            var data = enumerable.OrderBy(v => v).ToList();
            if (data.Count % 2 == 0) return (data[data.Count / 2 - 1] + data[data.Count / 2]) / 2;
            return data[data.Count / 2];
        }

        /// <summary>
        ///     Gets the standard deviation.
        /// </summary>
        /// <param name="values">The values.</param>
        /// <returns></returns>
        public static double GetStandardDeviation(IEnumerable<double> values)
        {
            var variance = GetVariance(values);
            return Math.Sqrt(variance);
        }

        /// <summary>
        ///     Gets the standard deviation per annum.
        /// </summary>
        /// <param name="values">The values.</param>
        /// <returns></returns>
        public static double? GetStandardDeviationPerAnnum(IDictionary<DateTime, double> values)
        {
            if (values == null) return null;
            if (values.Count < 30) return null;
            var timeSpan = values.Max(p => p.Key) - values.Min(p => p.Key);
            double days = values.Count;
            return GetStandardDeviation(values.Select(p => p.Value)) *
                   Math.Sqrt(MathematicalFunctions.SafeDivision(days, timeSpan.Days) * Constants.DAYS_OF_YEAR);
        }

        /// <summary>
        ///     Gets the performance.
        /// </summary>
        /// <param name="values">The values.</param>
        /// <returns></returns>
        public static double? GetPerformance(IEnumerable<double> values)
        {
            if (values == null) return null;
            if (!values.Any()) return null;
            if (values.First() == 0) return null;
            return values.Last() / values.First() - 1;
        }

        /// <summary>
        ///     Gets the variance.
        /// </summary>
        /// <param name="values">The values.</param>
        /// <returns></returns>
        public static double GetVariance(IEnumerable<double> values)
        {
            double variance = 0;
            if (values == null) return variance;
            if (!values.Any()) return variance;
            var count = Convert.ToDouble(values.Count());
            var average = values.Average();
            variance = values.Sum(value => Math.Pow(value - average, 2));
            return MathematicalFunctions.SafeDivision(variance, count);
        }

        /// <summary>
        ///     Gets the co variance.
        /// </summary>
        /// <param name="valuesX">The values_ X.</param>
        /// <param name="valuesY">The values_ Y.</param>
        /// <returns></returns>
        public static double GetCoVariance(IEnumerable<double> valuesX, IEnumerable<double> valuesY)
        {
            if (valuesX == null || valuesY == null || !valuesX.Any() || !valuesY.Any()) return 0;
            if (valuesX.Count() != valuesY.Count()) return 0;
            var count = valuesX.Count();
            var averageX = valuesX.Average();
            var averageY = valuesY.Average();

            double covariance = 0;
            for (var i = 0; i < count; i++)
                covariance += (valuesX.ElementAt(i) - averageX) * (valuesY.ElementAt(i) - averageY);

            return MathematicalFunctions.SafeDivision(covariance, Convert.ToDouble(count));
        }

        /// <summary>
        ///     Gets the correlation. (Pearson Method)
        /// </summary>
        /// <param name="valuesX">The values_ X.</param>
        /// <param name="valuesY">The values_ Y.</param>
        /// <returns></returns>
        public static double? GetCorrelation(IEnumerable<double> valuesX, IEnumerable<double> valuesY)
        {
            if (valuesX == null || valuesY == null || !valuesX.Any() || !valuesY.Any()) return null;

            var covariance = GetCoVariance(valuesX, valuesY);
            var stdDevX = GetStandardDeviation(valuesX);
            var stdDevY = GetStandardDeviation(valuesY);

            return MathematicalFunctions.SafeDivision(covariance, stdDevX * stdDevY);
        }

        /// <summary>
        ///     Gets the per annum performance.
        /// </summary>
        /// <param name="portfolioPerformance">The portfolio performance.</param>
        /// <param name="startDate">The start date.</param>
        /// <param name="endDate">The end date.</param>
        /// <returns></returns>
        public static double? GetPerAnnumPerformance(double? portfolioPerformance, DateTime startDate, DateTime endDate)
        {
            if (!portfolioPerformance.HasValue) return null;
            var duration = endDate - startDate;
            if (duration.Days < 367) return null;
            return portfolioPerformance / duration.Days * Constants.DAYS_OF_YEAR;
        }

        /// <summary>
        ///     Gets the item performance values.
        /// </summary>
        /// <param name="values">The values.</param>
        /// <returns></returns>
        public static IDictionary<DateTime, double> GetItemPerformanceValues(IDictionary<DateTime, double> values)
        {
            var result = new Dictionary<DateTime, double>();

            if (values == null) return result;
            if (values.Count <= 0) return result;

            var lastValue = values.First().Value;
            foreach (var valuePair in values)
            {
                result.Add(valuePair.Key, MathematicalFunctions.SafeDivision(valuePair.Value, lastValue) - 1);
                lastValue = valuePair.Value;
            }

            return result;
        }

        #endregion
    }
}