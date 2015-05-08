namespace Reflection4Net.Accessor
{
    public interface IPropertyAccessor<in S, R>
    {
        R Get(S obj);

        R Set(S obj, R value);
    }
}
