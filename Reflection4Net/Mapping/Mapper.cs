using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq.Expressions;
using Reflection4Net.Accessor;
using Reflection4Net.Extensions;

namespace Reflection4Net.Mapping
{
    public static class Mapper<TFrom, TTo>
    {
        private static Action<TFrom, TTo> _propertyMapper;

        public static void InitializePropertyMapper(Func<TFrom, string, object> memberAccessor, IDictionary<string, string> propertyNameMap = null)
        {
            propertyNameMap = propertyNameMap ?? typeof(TTo).GetPropertyMapBy<AdaptedNameAttribute>(a => a.Name);
            var from = Expression.Parameter(typeof(TFrom), "from");
            var to = Expression.Parameter(typeof(TTo), "to");
            var body = new List<Expression>();
            foreach(var targetProperty in typeof(TTo).GetProperties())
            {
                if (!targetProperty.CanWrite)
                    continue;
                var sourceProperty = typeof(TFrom).GetProperty(targetProperty.Name);
                if (sourceProperty == null || !sourceProperty.CanRead)
                    continue;
                var targetName = targetProperty.Name;
                var sourceName = propertyNameMap[targetName];
                if (String.IsNullOrEmpty(sourceName))
                    continue;
                Expression callToAccessor = null;
                if (memberAccessor != null)
                {
                    callToAccessor = Expression.Convert(Expression.Call(null, memberAccessor.Method, from, Expression.Constant(sourceName)), targetProperty.PropertyType);
                }
                else
                {
                    callToAccessor = Expression.Convert(Expression.PropertyOrField(from, sourceName), targetProperty.PropertyType);
                }
                body.Add(Expression.Assign(Expression.Property(to, targetName), callToAccessor));
            }

            Trace.WriteLine($"Generating Mapper from {body}");

            _propertyMapper = Expression.Lambda<Action<TFrom, TTo>>(Expression.Block(body), from, to).Compile();
        }

        public static Action<TFrom, TTo> PropertyMapper
        {
            get
            {
                if (_propertyMapper == null)
                {
                    InitializePropertyMapper(null);
                }

                return _propertyMapper;
            }
        }

        public static void Copy(TFrom from, TTo to)
        {
            PropertyMapper(from, to);
        }
    }


}
