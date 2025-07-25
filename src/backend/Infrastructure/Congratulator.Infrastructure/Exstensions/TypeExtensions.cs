namespace Congratulator.Infrastructure.Exstensions;

public static class TypeExtensions
{
    public static Type MakeGenericIfNeeded<T>(this Type t)
    {
        return t.IsGenericTypeDefinition
               ? t.MakeGenericType(typeof(T))
               : t;
    }
}
