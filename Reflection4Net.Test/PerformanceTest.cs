using Microsoft.VisualStudio.TestTools.UnitTesting;
using Reflection4Net.Test.Model;
using Reflection4Net.Test.Util;
using Reflection4Net.Accessor;
using Reflection4Net.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using System.Collections;
using System.Reflection;

namespace Reflection4Net.Test
{
    [TestClass]
    public class PerformanceTest
    {
        [TestMethod]
        public void CompareWithPureReflection()
        {
            var properties = new Dictionary<string, object>();
            properties.Add("Name", "Hugo");
            properties.Add("Id", "0");
            properties.Add("UpdateTime", DateTime.Now.Date);
            properties.Add("Summary", "description omit");
            var data = new DataModel();
            UpdatePropertyFromDictionary(data, properties);
            FillPropertyFromDictionary(data, properties);
            var mapper = ConvertFromDictionary(Mapper.CreateMap<IDictionary, DataModel>(), p => p);
            var times = 100000;
            var timeOfReflection = new TestTimer(() => UpdatePropertyFromDictionary(data, properties)).TimeForTimes(times);
            var timeOfLibrary = new TestTimer(() => FillPropertyFromDictionary(data, properties)).TimeForTimes(times);
            var timeOfMapper = new TestTimer(() => FillPropertyFromDictionaryByAutoMapper(data, properties)).TimeForTimes(times);

            Assert.IsTrue(timeOfReflection > timeOfLibrary);
            Assert.IsTrue(timeOfMapper > timeOfReflection);
        }

        protected T FillPropertyFromDictionaryByAutoMapper<T>(T data, Dictionary<string, object> properties)
        {
            var t = Mapper.Map<T>(properties);
            return t;
        }

        protected void FillPropertyFromDictionary<T>(T data, Dictionary<string, object> properties)
        {
            var accessor = data.GetAccessor();
            foreach(var pair in properties)
            {
                accessor.SetProperty(data, pair.Key, pair.Value);
            }
        }

        protected void UpdatePropertyFromDictionary<T>(T data, Dictionary<string, object> properties)
        {

            var type = data.GetType();
            foreach(var pair in properties)
            {
                var property = type.GetProperty(pair.Key);
                if (property != null)
                {
                    property.SetValue(data, pair.Value, null);
                }
            }
        }

        public static IMappingExpression<IDictionary, TDestination> ConvertFromDictionary<TDestination>(IMappingExpression<IDictionary, TDestination> exp, Func<string, string> propertyNameMapper)
        {
            foreach (PropertyInfo pi in typeof(TDestination).GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
            {
                string propertyName = pi.Name;
                propertyName = propertyNameMapper(propertyName);
                exp.ForMember(propertyName, cfg => cfg.MapFrom(r => r[propertyName]));
            }
            return exp;
        }
    }
}
