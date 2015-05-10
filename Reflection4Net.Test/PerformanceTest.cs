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
using System.Collections;
using System.Reflection;
using ROK.Reflection.FastMembers;
using Fasterflect;
using AMapper = AutoMapper.Mapper;
using Reflection4Net.Mapping;

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
            FillPropertyFromDictionaryByReflection(data, properties);
            FillPropertyFromDictionaryByLibrary(data, properties);
            var mapper = ConvertFromDictionary(AMapper.CreateMap<IDictionary, DataModel>(), p => p);
            Mapper<Dictionary<string, object>, DataModel>.InitializePropertyMapper((dict, name) =>
            {
                object value;
                if (dict.TryGetValue(name, out value))
                    return value;
                else
                    return null;
            });
            var times = 100000;
            var timeOfDirect = new TestTimer(n => FillPropertyFromDictionaryByDirectCalls(data, properties)).TimeForTimes(times);
            var timeOfReflection = new TestTimer(n => FillPropertyFromDictionaryByReflection(data, properties)).TimeForTimes(times);
            var timeOfLibrary = new TestTimer(n => FillPropertyFromDictionaryByLibrary(data, properties)).TimeForTimes(times);
            var timeOfMyMapper = new TestTimer(n => FillPropertyFromDictionaryByMyMapper(data, properties)).TimeForTimes(times);
            var timeOfMapper = new TestTimer(n => FillPropertyFromDictionaryByAutoMapper(data, properties)).TimeForTimes(times);
            var timeOfFastMember = new TestTimer(n => FillPropertyFromDictionaryByFastMembers(data, properties)).TimeForTimes(times);
            var timeOfFasterflect = new TestTimer(n => FillPropertyFromDictionaryByFasterflect(data, properties)).TimeForTimes(times);
            
            Assert.IsTrue(timeOfReflection > timeOfLibrary);
            Assert.IsTrue(timeOfMapper > timeOfReflection);
        }

        protected DataModel FillPropertyFromDictionaryByDirectCalls(DataModel data, Dictionary<string, object> properties)
        {
            data.Name = properties["Name"] as string;
            data.Id = properties["Id"] as string;
            data.UpdateTime = (DateTime)properties["UpdateTime"];
            data.Summary = properties["Summary"] as string;
            return data;
        }

        protected T FillPropertyFromDictionaryByAutoMapper<T>(T data, Dictionary<string, object> properties)
        {
            return AMapper.Map<T>(properties);
        }

        protected T FillPropertyFromDictionaryByMyMapper<T>(T data, Dictionary<string, object> properties)
        {
            Mapper<Dictionary<string, object>, T>.Copy(properties, data);
            return data;
        }

        protected void FillPropertyFromDictionaryByLibrary<T>(T data, Dictionary<string, object> properties)
        {
            var accessor = data.GetAccessor();
            foreach(var pair in properties)
            {
                accessor.SetProperty(data, pair.Key, pair.Value);
            }
        }

        protected void FillPropertyFromDictionaryByFastMembers<T>(T data, Dictionary<string, object> properties)
        {
            var fastSetters = typeof(T).GetFastSetters().ToDictionary(s => s.Name, s => s);
            foreach (var pair in properties)
            {
                IFastSetter setter;
                if (fastSetters.TryGetValue(pair.Key, out setter))
                {
                    setter.SetValue(data, pair.Value);
                }
            }
        }

        protected void FillPropertyFromDictionaryByFasterflect<T>(T data, Dictionary<string, object> properties)
        {
            
            foreach (var pair in properties)
            {
                data.SetPropertyValue(pair.Key, pair.Value);
            }
        }

        protected void FillPropertyFromDictionaryByReflection<T>(T data, Dictionary<string, object> properties)
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

        public static AutoMapper.IMappingExpression<IDictionary, TDestination> ConvertFromDictionary<TDestination>(AutoMapper.IMappingExpression<IDictionary, TDestination> exp, Func<string, string> propertyNameMapper)
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
