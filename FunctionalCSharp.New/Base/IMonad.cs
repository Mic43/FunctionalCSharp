namespace FunctionalCSharp.New.Base;

public interface IMonad<TMonad> : IApplicative<TMonad> where TMonad : IMonad<TMonad>
{
    public new static IKind<TMonad, V> Map<T, V>(IKind<TMonad, T> f, Func<T, V> fun)
        => TMonad.Bind(f, t => TMonad.Pure(fun(t)));

    public new static IKind<TMonad, V> Apply<T, V>(IKind<TMonad, T> applicative, IKind<TMonad, Func<T, V>> fun)
        => TMonad.Bind(applicative, t => TMonad.Map(fun, f => f(t)));

    public static IKind<TMonad, T> Join<T>(IKind<TMonad, IKind<TMonad, T>> monad)
        => TMonad.Bind(monad, innerMonad => innerMonad);

    public static abstract IKind<TMonad, V> Bind<T, V>(IKind<TMonad, T> monad, Func<T, IKind<TMonad, V>> fun);
}

public static class MonadExt
{
    public static IKind<TMonad, Z> SelectMany<TMonad, T, V, Z>(this IKind<TMonad, T> monad,
        Func<T, IKind<TMonad, V>> binder,
        Func<T, V, Z> projection) where TMonad : IMonad<TMonad> =>
        TMonad.Bind(monad,
            t => binder(t).Select(v => projection(t, v)));

    public static IKind<TMonad, V> Select<TMonad, T, V>(this IKind<TMonad, T> monad, Func<T, V> mapper)
        where TMonad : IMonad<TMonad> =>
        TMonad.Map(monad, mapper);
}