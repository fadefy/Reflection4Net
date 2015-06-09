using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Reflection4Net.Extensions
{
    public static class ExpressionExtensions
    {
        public static MethodInfo InstanceEqual = typeof(Object).GetMethod("Equals", BindingFlags.Static | BindingFlags.Public);

        public static UnaryExpression GetPropertyOrFieldAsType<T>(this Expression instance, string name)
        {
            return Expression.Convert(Expression.PropertyOrField(instance, name), typeof(T));
        }

        public static MethodCallExpression CallWith(this MethodInfo method, params Expression[] arguments)
        {
            return Expression.Call(method, arguments);
        }

        public static MethodCallExpression CallOn(this MethodInfo method, Expression instance, params Expression[] arguments)
        {
            return Expression.Call(instance, method, arguments);
        }

        public static T CompileTo<T>(this Expression expression, params ParameterExpression[] parameters)
        {
            return Expression.Lambda<T>(expression, parameters).Compile();
        }

        public static ParameterExpression Parameter(this Type type, string name = null)
        {
            return Expression.Parameter(type, name);
        }

        public static BinaryExpression AndAlso(this Expression left, Expression right)
        {
            return Expression.AndAlso(left, right);
        }

        public static BinaryExpression ExclusiveOr(this Expression left, Expression right)
        {
            return Expression.ExclusiveOr(left, right);
        }

        public static BinaryExpression Coalesce(this Expression expression, Expression nullObjectExpression)
        {
            return Expression.Coalesce(expression, nullObjectExpression);
        }

        public static Expression EqualTo(this Expression expression, object value)
        {
            return InstanceEqual.CallWith(expression, Expression.Constant(value));
        }

        public static Expression EqualTo(this Expression left, Expression right)
        {
            return InstanceEqual.CallWith(left, right);
        }

        public static Expression NotEqualTo(this Expression expression, object value)
        {
            return Expression.Not(expression.EqualTo(value));
        }
    }
}
