namespace FunctionalCSharp.New.Monads;

public sealed record ReaderT<TEnv, TMonad, T>(Func<TEnv, IKind<TMonad, T>> RunReaderT)
    : IKind<ReaderT<TEnv, TMonad>, T> where TMonad : IMonad<TMonad>;

public sealed class ReaderT<TEnv, TMonad> : IMonad<ReaderT<TEnv, TMonad>> where TMonad : IMonad<TMonad>
{
    public static IKind<ReaderT<TEnv, TMonad>, V> Map<T, V>(IKind<ReaderT<TEnv, TMonad>, T> f, Func<T, V> fun) =>
        IMonad<ReaderT<TEnv, TMonad>>.Map(f, fun);

    public static IKind<ReaderT<TEnv, TMonad>, V> Apply<T, V>(IKind<ReaderT<TEnv, TMonad>, T> applicative,
        IKind<ReaderT<TEnv, TMonad>, Func<T, V>> fun) =>
        IMonad<ReaderT<TEnv, TMonad>>.Apply(applicative, fun);

    public static IKind<ReaderT<TEnv, TMonad>, V> Bind<T, V>(IKind<ReaderT<TEnv, TMonad>, T> monad,
        Func<T, IKind<ReaderT<TEnv, TMonad>, V>> fun)
    {
        var readerT = monad.To();
        return new ReaderT<TEnv, TMonad, V>(env =>
        {
            var resT = readerT.RunReaderT(env);
            return TMonad.Bind(resT, t => fun(t).To().RunReaderT(env));
        });
    }

    public static IKind<ReaderT<TEnv, TMonad>, T> Pure<T>(T value) =>
        new ReaderT<TEnv, TMonad, T>(_ => TMonad.Pure(value));
}

public static class ReaderTExt
{
    // public static MaybeT<TMonad, Z> SelectMany<T, V, Z, TMonad>(this MaybeT<TMonad, T> maybeT,
    //     Func<T, MaybeT<TMonad, V>> binder,
    //     Func<T, V, Z> projection) where TMonad : IMonad<TMonad>
    // {
    //     return MaybeT<TMonad>
    //         .Bind(maybeT, t => MaybeT<TMonad>.Bind(binder(t), v => MaybeT<TMonad>.Pure(projection(t, v)))).To();
    // }
    //
    // public static MaybeT<TMonad, V> Select<T, V, TMonad>(this MaybeT<TMonad, T> maybeT, Func<T, V> mapper)
    //     where TMonad : IMonad<TMonad>
    // {
    //     return MaybeT<TMonad>.Map(maybeT, mapper).To();
    // }
    //
    public static ReaderT<TEnv, TMonad, T> To<TEnv, TMonad, T>(this IKind<ReaderT<TEnv, TMonad>, T> kind)
        where TMonad : IMonad<TMonad>
    {
        return (ReaderT<TEnv, TMonad, T>)kind;
    }
}

