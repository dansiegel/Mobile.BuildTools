using System;

namespace Mobile.BuildTools.Drawing
{
    public static class FloatExtensions
    {
        public static bool IsEqualTo(this float value, float comparisonValue) => Math.Abs(value - comparisonValue) <= 0.0001;
    }
}
