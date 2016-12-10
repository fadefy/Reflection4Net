using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Reflection4Net.Extensions;
using Reflection4Net.Test.Util;

namespace Reflection4Net.Test.Extensions
{
    [TestClass]
    public class FunctionExtensionTests
    {
        [TestMethod]
        [TestCategory("Performance")]
        public void PerformanceTestOfCompiledExpressionAndDynamicInvoke()
        {
            Action<string, string> someAction = (a, b) => { a = b; };
            var compiledVersion = someAction.CastToGenericAction<string, object>();
            var dynamicVersion = someAction.CastAsAction<string, object>();

            var perfRace = new Dictionary<string, Action<long>> {
                ["Compiled"] = n => compiledVersion("Hugo", "Gu"),
                ["Dynamic"] = n => dynamicVersion("Hugo", "Gu"),
            }.ComparePerformance(1000000);

            Assert.IsTrue(perfRace["Compiled"] < perfRace["Dynamic"]);
        }
    }
}
