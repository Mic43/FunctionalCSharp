using FunctionalCSharp.New.Base;

namespace FunctionalCSharp.New;

public static class MonadicConditionals
{
    public static IKind<TMonadPlus, T> When<TMonadPlus, T>(bool condition, IKind<TMonadPlus, T> monad)
        where TMonadPlus : IMonadPlus<TMonadPlus> =>
        condition ? monad : TMonadPlus.Empty<T>();

    public static IKind<TMonadPlus, T> Try<TMonadPlus, T, TException>(Func<IKind<TMonadPlus, T>> monadFunc)
        where TMonadPlus : IMonadPlus<TMonadPlus> where TException : Exception
    {
        try
        {
            return monadFunc();
        }
        catch (TException _)
        {
            return TMonadPlus.Empty<T>();
        }
    }
    public static IKind<TMonadPlus, T> Try<TMonadPlus, T>(Func<IKind<TMonadPlus, T>> monadFunc)
        where TMonadPlus : IMonadPlus<TMonadPlus>
        => Try<TMonadPlus, T, Exception>(monadFunc);
}