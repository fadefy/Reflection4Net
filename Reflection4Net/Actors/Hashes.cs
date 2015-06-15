using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Reflection4Net.Extensions;

namespace Reflection4Net.Actors
{
    public class Hashes<T> : AbstractOperation<Func<T, int>>
    {
        public override Func<T, int> Build(ITypeMappingInfoProvider infoProvider)
        {
            var instance = typeof(T).Parameter("instance");
            var hashValues = from member in infoProvider.GetOperandMembers(typeof(T), typeof(T))
                             let memberValue = instance.GetPropertyOrFieldAsType<Object>(member.Name).Coalesce()
                             select memberValue.HashCode();
            var aggregatedHash = hashValues.Aggregate((result, hash) => result.ExclusiveOr(hash));

            Trace.Write(String.Format("Generated GetHash method for type {0}: {1}", typeof(T), aggregatedHash));

            return aggregatedHash.CompileTo<Func<T, int>>(instance);
        }
    }
}
