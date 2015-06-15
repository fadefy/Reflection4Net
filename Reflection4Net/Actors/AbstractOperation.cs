using System;

namespace Reflection4Net.Actors
{
    public abstract class AbstractOperation<T> : IOperation<T>
    {
        protected T _builtOperation = default(T);

        public abstract T Build(ITypeMappingInfoProvider infoProvider);

        public virtual T Operation
        {
            get
            {
                if (Equals(_builtOperation, default(T)))
                    throw new InvalidOperationException("The Operation hasn't been built yet.");

                return _builtOperation;
            }
        }

        public virtual object Invoke(params object[] parameters)
        {
            var delegation = Operation as Delegate;
            if (delegation == null)
                new InvalidOperationException("Operation is not an Delegate. Override Invoke method to execute your custom operation by yourself.");

            return delegation.DynamicInvoke(parameters);
        }
    }
}
