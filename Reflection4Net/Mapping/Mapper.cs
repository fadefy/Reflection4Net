using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Reflection4Net.Extensions;
using Reflection4Net.Accessor;
using System.Linq.Expressions;

namespace Reflection4Net.Mapping
{
    public class Mapper<TFrom, TTo>
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
                var targetName = targetProperty.Name;
                var sourceName = propertyNameMap[targetName];
                if (String.IsNullOrEmpty(sourceName))
                    continue;
                var callToAccessor = Expression.Convert(Expression.Call(memberAccessor.Method, from, Expression.Constant(sourceName)), targetProperty.PropertyType);
                body.Add(Expression.Assign(Expression.Property(to, targetName), callToAccessor));
            }

            _propertyMapper = Expression.Lambda<Action<TFrom, TTo>>(Expression.Block(body), from, to).Compile();
        }

        public static Action<TFrom, TTo> PropertyMapper
        {
            get { return _propertyMapper; }
        }

        public static void Copy(TFrom from, TTo to)
        {
            if (_propertyMapper == null)
                throw new InvalidOperationException("Mapper hasn't initilized.");

            _propertyMapper(from, to);
        }
    }


}
