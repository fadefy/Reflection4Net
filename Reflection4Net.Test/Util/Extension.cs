using System;

namespace Reflection4Net.Test.Util
{
    public static class Extension
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ToNullableString(this object value)
        {
            return value == null ? null : Convert.ToString(value);
        }
    }
}
