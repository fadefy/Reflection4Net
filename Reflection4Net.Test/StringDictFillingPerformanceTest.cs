using Microsoft.VisualStudio.TestTools.UnitTesting;
using Reflection4Net.Test.Model;
using Reflection4Net.Test.Util;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Reflection4Net.Test
{
    [TestClass]
    public class StringDictFillingPerformanceTest
    {
        [TestMethod]
        public void FillPropertyFromStringDict()
        {
            var stringDict = new Dictionary<string, string>();

            stringDict.Add("Name", "Hugo");
            stringDict.Add("Id", "0");
            stringDict.Add("UpdateTime", DateTime.Now.Date.ToString());
            stringDict.Add("Summary", "description omit");

            var data = new DataModel();

            var times = 100000;
            var timeOfReflection = new TestTimer(n => FillByReflection(stringDict, data)).TimeForTimes(times);

            Assert.IsNotNull(timeOfReflection);
        }

        protected void FillByReflection(IDictionary<string, string> properties, DataModel data)
        {
            var type = data.GetType();
            foreach(var pair in properties)
            {
                var propertyInfo = type.GetProperty(pair.Key);
                if (propertyInfo != null)
                {
                    propertyInfo.SetValue(data, Convert(pair.Value, propertyInfo.PropertyType), null);
                }
            }
        }

        protected object Convert(string value, Type type)
        {
            var converter = TypeDescriptor.GetConverter(type);
            if (converter.CanConvertFrom(typeof(string)))
            {
                return converter.ConvertFrom(value);
            }
            return null;
        }
    }
}
