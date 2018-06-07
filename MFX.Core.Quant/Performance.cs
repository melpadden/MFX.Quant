using System;
using System.Collections.Generic;
using System.Linq;
using MFX.Core.Quant.Interfaces;
using MFX.Core.Quant.Models;

namespace MFX.Core.Quant
{
    public static class Performance
    {
        /// <summary>
        ///     Gets the N days performance.
        /// </summary>
        /// <param name="performanceItems">The performance items.</param>
        /// <param name="nDays">The n days.</param>
        /// <returns></returns>
        public static IEnumerable<IPerformanceItem> GetNDaysPerformance(IEnumerable<IPerformanceItem> performanceItems,
            int nDays)
        {
            var list = performanceItems
                .Where(i => i.Value.HasValue)
                .ToList();

            var result = new List<IPerformanceItem>();

            for (var i = nDays; i < list.Count; i++)
            {
                var date = list[i].Date;
                var v = list[i].Value.GetValueOrDefault(0);
                var p = list[i - nDays].Value.GetValueOrDefault(0);
                var value = p == 0 ? 0 : v / p - 1;
                result.Add(new PerformanceItem(date, value));
            }

            return result;
        }

        /// <summary>
        ///     Gets the N days performance.
        /// </summary>
        /// <param name="indexItems">The performance items.</param>
        /// <param name="nDays">The n days.</param>
        /// <returns></returns>
        public static IEnumerable<double> GetNDaysPerformance(IEnumerable<double> indexItems, int nDays)
        {
            var list = indexItems
                .ToList();

            var result = new List<double>();

            for (var i = nDays; i < list.Count; i++)
            {
                var v = list[i];
                var p = list[i - nDays];
                var value = p == 0 ? 0 : v / p - 1;
                result.Add(value);
            }

            return result;
        }

        /// <summary>
        ///     Converts performance values to index values.
        /// </summary>
        /// <param name="performanceValues">The performance values</param>
        /// <param name="startValue">The start value</param>
        /// <returns>The indexed performance values</returns>
        public static IDictionary<DateTime, double> PerformanceValuesToIndex(
            IDictionary<DateTime, double> performanceValues, double startValue = 1)
        {
            var result = new Dictionary<DateTime, double>();

            if (performanceValues != null)
                if (performanceValues.Count > 0)
                {
                    double previousIndexValue = 1;
                    foreach (var kvp in performanceValues)
                    {
                        double currentIndexValue;
                        if (kvp.Key == performanceValues.First().Key)
                            currentIndexValue = startValue;
                        else
                            currentIndexValue = previousIndexValue * (1 + kvp.Value);

                        result.Add(kvp.Key, currentIndexValue);
                        previousIndexValue = currentIndexValue;
                    }
                }

            return result;
        }

        /// <summary>
        ///     Gets a calibrated performance index from the <paramref name="performanceValues" />
        ///     using the <paramref name="sourceIndex" />'s period bounds and index start value.
        /// </summary>
        /// <param name="sourceIndex">The source index</param>
        /// <param name="performanceValues">The performance values</param>
        /// <param name="compressSubPeriods">
        ///     If <code>true</code>, the sub-periods will match
        ///     the frequency of the <paramref name="sourceIndex" />'s periods.
        /// </param>
        /// <returns>The indexed performance values.</returns>
        public static IDictionary<DateTime, double> GetCalibratedIndex(IDictionary<DateTime, double> sourceIndex,
            IDictionary<DateTime, double> performanceValues, bool compressSubPeriods)
        {
            if (performanceValues == null) throw new ArgumentNullException("performanceValues");

            if (sourceIndex == null) return null;

            var result = new Dictionary<DateTime, double>();
            if (performanceValues.Count == 0) return result;

            // Make sure both inputs are in ascending order
            performanceValues = performanceValues.OrderBy(x => x.Key).ToDictionary(k => k.Key, v => v.Value);
            var isDescending = sourceIndex.First().Key > sourceIndex.Last().Key;
            if (isDescending) sourceIndex = sourceIndex.OrderBy(x => x.Key).ToDictionary(k => k.Key, v => v.Value);

            var sourceIndexInRange = sourceIndex.Where(x =>
                x.Key >= performanceValues.First().Key && x.Key <= performanceValues.Last().Key);
            if (!sourceIndexInRange.Any()) return result;

            var previousDate = sourceIndexInRange.First().Key;
            var idx = sourceIndexInRange.First().Value;
            var subPeriods = new Dictionary<DateTime, double>();

            foreach (var sourceItem in sourceIndexInRange)
            {
                subPeriods.Clear();
                foreach (var item in performanceValues.Where(x =>
                    x.Key > previousDate && x.Key <= sourceItem.Key))
                {
                    idx *= 1 + item.Value;
                    subPeriods.Add(item.Key, idx);
                }

                if (compressSubPeriods)
                    result.Add(sourceItem.Key, idx);
                else
                    foreach (var subItem in subPeriods)
                        result.Add(subItem.Key, subItem.Value);

                previousDate = sourceItem.Key;
            }

            return isDescending ? result.OrderByDescending(k => k.Key).ToDictionary(k => k.Key, k => k.Value) : result;
        }

        /// <summary>
        ///     Calculates the average amount of values per year of a Performance Time line.
        ///     Used for statistic-ratio functions.
        /// </summary>
        /// <param name="valuesCount">The values count.</param>
        /// <param name="periodDays">The period days.</param>
        /// <returns></returns>
        public static double? CalculateValuesPerYear(int valuesCount, int periodDays)
        {
            if (periodDays <= 0) return null;

            return (double) valuesCount / periodDays * Constants.DAYS_OF_YEAR;
        }

        /// <summary>
        ///     Gets the overall performance of a performance time line.
        /// </summary>
        /// <param name="performanceItems">The performance items.</param>
        /// <returns></returns>
        public static double? GetPerformance(IEnumerable<IPerformanceItem> performanceItems)
        {
            var firstItem = performanceItems.FirstOrDefault();
            var lastItem = performanceItems.LastOrDefault();

            if (firstItem == null || lastItem == null) return null;

            if (!firstItem.Value.HasValue) return null;

            if (!lastItem.Value.HasValue) return null;

            return lastItem.Value.Value / firstItem.Value.Value - 1;
        }

        /// <summary>
        ///     Gets the performance per annum of a performance time line.
        ///     Would be pointless for a period which is less than a year, so in that case
        ///     this method returns null.
        /// </summary>
        /// <param name="fullPerformance">The full performance.</param>
        /// <param name="periodDays">The period days.</param>
        /// <returns></returns>
        public static double? GetPerformancePerAnnum(double fullPerformance, int periodDays)
        {
            if (periodDays == 0) return null;
            if (periodDays < Constants.DAYS_OF_YEAR - 0.25) return null;

            return Math.Pow(1 + fullPerformance, Constants.DAYS_OF_YEAR / periodDays) - 1;
        }


        /// <summary>
        ///     Gets the jensen alpha for a performance time line compared to another.
        /// </summary>
        /// <param name="performancePerAnnum">The performance per annum.</param>
        /// <param name="comparePerformancePerAnnum">The compare performance per annum.</param>
        /// <param name="riskFreeInterestRate">The risk free interest rate.</param>
        /// <param name="beta">The beta.</param>
        /// <returns></returns>
        public static double? GetJensenAlpha(double performancePerAnnum, double comparePerformancePerAnnum,
            double riskFreeInterestRate, double beta)
        {
            var subTerm = beta * (comparePerformancePerAnnum - riskFreeInterestRate);
            return performancePerAnnum - subTerm - riskFreeInterestRate;
        }


        public static IDictionary<DateTime, double> GetEntryFromIndexPerformance(IEnumerable<IPerformanceItem> data)
        {
            IDictionary<DateTime, double> result = new Dictionary<DateTime, double>();
            var index = 0;
            double lastIndex = 100;
            foreach (var item in data.Where(x => x.Value.HasValue).OrderBy(x => x.Date))
            {
                result.Add(
                    item.Date,
                    index == 0 ? 0 : MathematicalFunctions.SafeDivision(item.Value ?? 100, lastIndex) - 1);
                lastIndex = item.Value ?? 100;
                index++;
            }

            return result;
        }

        public static IDictionary<DateTime, double> GetIndexFromEntryPerformance(IEnumerable<IPerformanceItem> data)
        {
            IDictionary<DateTime, double> result = new Dictionary<DateTime, double>();
            double index = 100;
            foreach (var item in data.Where(x => x.Value.HasValue).OrderBy(x => x.Date))
            {
                index *= 1 + (item.Value ?? 0);

                result.Add(item.Date, index);
            }

            return result;
        }

        public static double? GetRiskPerAnnumWorkdays(this IEnumerable<double> values)
        {
            if (values == null) return null;
            return values.StandardDeviation() * Math.Pow(260, 0.5);
        }
    }
}