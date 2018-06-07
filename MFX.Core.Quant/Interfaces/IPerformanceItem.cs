using System;

namespace MFX.Core.Quant.Interfaces
{
    /// <summary>
    ///     Represents a statment of the value of an asset at a given date
    /// </summary>
    public interface IPerformanceItem
    {
        /// <summary>
        ///     Gets or sets the date.
        /// </summary>
        /// <value>The date.</value>
        DateTime Date { get; set; }

        /// <summary>
        ///     Gets or sets the value.
        /// </summary>
        /// <value>The value.</value>
        double? Value { get; set; }
    }
}