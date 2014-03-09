using Microsoft.VisualStudio.TestTools.UnitTesting;
using Reflection4Net.Accessor;
using Reflection4Net.Extension;
using Reflection4Net.Test.Model;
using Reflection4Net.Test.Util;
using System;

namespace Reflection4Net.Test
{
    [TestClass]
    public class MultiThreadPerformanceTest
    {
        [TestMethod]
        [TestCategory("Performance")]
        public void GetPropertyPerformanceTest()
        {
            var time = 10000000;
            var name = "Dummy";
            var model = new DataModel() { Name = name };
            var classSpecifiedAdapter = AdaptedAccessorFactory.Instance.GetAccessor(model);
            var globalTarget = String.Empty;

            var timeDynamic = new TestTimer(() =>
            {
                globalTarget = classSpecifiedAdapter.GetProperty(model, "Title").ToNullableString();
            }).TimeForTimes(time);
            Assert.AreEqual(name, globalTarget);

            var timeStatic = new TestTimer(() =>
            {
                globalTarget = model.Name;
            }).TimeForTimes(time);
            Assert.AreEqual(name, globalTarget);

            var timeRandomClass = new TestTimer(() =>
            {
                globalTarget = model.GetProperty("Title").ToNullableString();
            }).TimeForTimes(time);
            Assert.AreEqual(name, globalTarget);

            Assert.IsTrue(timeDynamic > timeStatic);
            Assert.IsTrue(timeRandomClass > timeDynamic);

            Assert.IsTrue(timeDynamic.TotalMilliseconds < timeStatic.TotalMilliseconds * 10);
            Assert.IsTrue(timeRandomClass.TotalMilliseconds < timeStatic.TotalMilliseconds * 25);
        }

        [TestMethod]
        [TestCategory("Performance")]
        public void MultiThreadGetPropertyPerformanceTest()
        {
            var time = 20000000;
            var name = "Dummy";
            var model = new DataModel() { Name = name };
            var classSpecifiedAdapter = AdaptedAccessorFactory.Instance.GetAccessor(model);
            var globalTarget = String.Empty;

            var timeDynamic = new TestTimer(() =>
            {
                globalTarget = classSpecifiedAdapter.GetProperty(model, "Title").ToNullableString();
            }).TimeForTimes(time);
            Assert.AreEqual(name, globalTarget);

            var timeDynamicMT = new TestTimer(() =>
            {
                globalTarget = classSpecifiedAdapter.GetProperty(model, "Title").ToNullableString();
            }).TimeForTimesParallel(time, 4);
            Assert.AreEqual(name, globalTarget);

            // The performance is not improved too much by using multi-thread.
            Assert.IsTrue(timeDynamic.TotalMilliseconds > timeDynamicMT.TotalMilliseconds * 1.5);
        }
    }
}
