using System.Collections.Generic;

namespace MFX.Core.Quant
{
    public class InformationRatio
    {
        public static double? CalculateInformationRatio(
            IEnumerable<double> dailyPerformance1,
            IEnumerable<double> dailyPerformance2)
        {
            double? informationRatio;
            TrackingError.CalculateTrackingError(dailyPerformance1, dailyPerformance2, out informationRatio);
            return informationRatio;
        }

        /// <summary>
        ///     Gets the information ratio for a performance time line compared to another.
        /// </summary>
        /// <param name="performancePerAnnum">The performance per annum.</param>
        /// <param name="comparePerformancePerAnnum">The compare performance per annum.</param>
        /// <param name="trackingError">The tracking error.</param>
        /// <returns></returns>
        public static double? GetInformationRatio(double performancePerAnnum, double comparePerformancePerAnnum,
            double trackingError)
        {
            if (trackingError == 0) return null;

            return (performancePerAnnum - comparePerformancePerAnnum) / trackingError;
        }
    }
}