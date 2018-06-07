using System;
using System.Collections.Generic;
using System.Linq;
using MFX.Core.Quant.Interfaces;

namespace MFX.Core.Quant
{
    /// <summary>
    ///     value At Risk calculations
    /// </summary>
    public class Var
    {
        /// <summary>
        ///     Gets the value at risk confidence level.
        /// </summary>
        /// <param name="nDaysPerformanceItems">The n days performance items.</param>
        /// <param name="confidenceLevel">The confidence level.</param>
        /// <returns></returns>
        public static double GetValueAtRiskConfidenceLevel(IEnumerable<IPerformanceItem> nDaysPerformanceItems,
            double confidenceLevel)
        {
            var performaceValues = nDaysPerformanceItems
                .Where(d => d.Value.HasValue)
                .Select(d => d.Value.GetValueOrDefault(0))
                .OrderBy(d => d)
                .ToList();

            if (performaceValues.Count == 0) return 0;

            var index = Convert.ToInt32(Math.Ceiling(performaceValues.Count * (1 - confidenceLevel)));

            return performaceValues[index - 1];
        }

        public static double GetValueAtRiskConfidenceLevel(IEnumerable<double> nDaysPerformanceItems,
            double confidenceLevel)
        {
            var performaceValues = nDaysPerformanceItems
                .OrderBy(d => d)
                .ToList();

            if (performaceValues.Count == 0) return 0;

            var index = Convert.ToInt32(Math.Ceiling(performaceValues.Count * (1 - confidenceLevel)));

            return performaceValues[index - 1];
        }

        /// <summary>
        ///     Gets the value at risk chart data.
        /// </summary>
        /// <param name="nDaysPerformanceItems">The n days performance items.</param>
        /// <returns></returns>
        public static Dictionary<double, int> GetValueAtRiskChartData(
            IEnumerable<IPerformanceItem> nDaysPerformanceItems)
        {
            var data = nDaysPerformanceItems
                .Where(x => x.Value.HasValue)
                .Select(x => Convert.ToInt32(x.Value.GetValueOrDefault(0) * 100))
                .GroupBy(x => x)
                .ToDictionary(x => x.Key, x => x.Count());

            if (data.Count == 0) return new Dictionary<double, int>();

            var min = data.Min(x => x.Key);
            var max = data.Max(x => x.Key);

            for (var i = min; i < max; i++)
                if (!data.ContainsKey(i))
                    data.Add(i, 0);

            return data.ToDictionary(i => (double) i.Key / 100, i => i.Value);
        }
    }
}