using System;
using MFX.Core.Quant.Interfaces;

namespace MFX.Core.Quant.Models
{
    public struct PerformanceItem : IPerformanceItem
    {
        public PerformanceItem(DateTime date, double? value)
        {
            Date = date;
            Value = value;
        }

        public DateTime Date { get; set; }
        public double? Value { get; set; }
    }
}