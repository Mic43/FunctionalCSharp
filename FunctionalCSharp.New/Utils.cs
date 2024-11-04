using FunctionalCSharp.New.Monads;

namespace FunctionalCSharp.New
{
    static class Utils
    {
        public static Func<T, T> Id<T>() => a => a;

        public static Func<T1, Func<T2, TRes>> Curry<T1, T2, TRes>(Func<T1, T2, TRes> f) =>
            t => t2 => f(t, t2);
    }
}