using System;
using System.Linq.Expressions;

namespace Reflection4Net.Extensions
{
    public static class ArgumentsGuards
    {
        public static void NotNull(Expression<Func<object>> argument)
        {
            var value = argument.GetMemberValue();
            if (value == null)
                throw new ArgumentNullException(argument.GetMemberName());
        }
    }
}
