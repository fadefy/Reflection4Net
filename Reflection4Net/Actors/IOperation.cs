using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reflection4Net.Actors
{
    public interface IOperation<D>
    {
        D Build(ITypeMappingInfoProvider infoProvider);

        D Operation { get; }

        object Invoke(params object[] parameters);
    }
}
