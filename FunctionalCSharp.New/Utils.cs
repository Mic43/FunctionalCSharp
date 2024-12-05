using FunctionalCSharp.New.Monads;

namespace FunctionalCSharp.New;

public static class Utils
{
    public static Func<T, T> Id<T>() => a => a;

    public static Func<T1, Func<T2, TRes>> Curry<T1, T2, TRes>(this Func<T1, T2, TRes> f) =>
        t => t2 => f(t, t2);

    public static Func<T, Z> Compose<T, V, Z>(this Func<T, V> a, Func<V, Z> b) => t => b(a(t));

    public static T Log<T>(T value)
    {
        Console.WriteLine(value);
        return value;
    }
}