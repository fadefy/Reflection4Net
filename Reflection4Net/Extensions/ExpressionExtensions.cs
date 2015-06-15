using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace Reflection4Net.Extensions
{
    public static class ExpressionExtensions
    {
        public static MethodInfo InstanceEqual = typeof(Object).GetMethod("Equals", BindingFlags.Static | BindingFlags.Public);
        public static MethodInfo ReferenceEqual = typeof(Object).GetMethod("ReferenceEquals", BindingFlags.Static | BindingFlags.Public);
        public static MethodInfo GetHash = typeof(Object).GetMethod("GetHashCode", BindingFlags.Instance | BindingFlags.Public);
        public static Expression NullObject = Expression.Constant(new object());

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

        public static BinaryExpression Coalesce(this Expression expression, Expression nullObjectExpression = null)
        {
            return Expression.Coalesce(expression, nullObjectExpression ?? NullObject);
        }

        public static Expression HashCode(this Expression expression)
        {
            return GetHash.CallOn(expression);
        }

        public static Expression EqualTo(this Expression expression, object value)
        {
            return InstanceEqual.CallWith(expression, Expression.Constant(value));
        }

        public static Expression ReferenceEqualTo(this Expression left, Expression right)
        {
            return ReferenceEqual.CallWith(left, right);
        }

        public static Expression EqualTo(this Expression left, Expression right)
        {
            return InstanceEqual.CallWith(left, right);
        }

        public static Expression NotEqualTo(this Expression expression, object value)
        {
            return Expression.Not(expression.EqualTo(value));
        }

        public static Expression Throw(this Exception exception)
        {
            return Expression.Throw(Expression.Constant(exception));
        }

        public static Expression AssignFrom(this Expression left, Expression right)
        {
            return Expression.Assign(left, right);
        }

        public static Expression Then(this Expression condition, Expression ifTrue)
        {
            return Expression.IfThen(condition, ifTrue);
        }

        public static IEnumerable<Expression> Concat(this Expression first, Expression second)
        {
            return new[] { first, second };
        }

        public static Expression AsABlock(this IEnumerable<Expression> expresssions, params ParameterExpression[] variables)
        {
            return Expression.Block(variables, expresssions);
        }
    }
}
