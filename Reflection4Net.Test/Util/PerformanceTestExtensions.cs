using System;
using System.Collections.Generic;
using System.Linq;

namespace Reflection4Net.Test.Util
{
    public static class PerformanceTestExtensions
    {
        public static Dictionary<string, TimeSpan> ComparePerformance(this IDictionary<string, Action<long>> actions, int times)
        {
            return actions.ToDictionary(p => p.Key, p => new TestTimer(p.Value).TimeForTimes(times));
        }
    }
}
