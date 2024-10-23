namespace FunctionalCSharp.New.Monads;

public sealed record MaybeT<TMonad, T>(IKind<TMonad, Maybe<T>> InnerMonad)
    : IKind<MaybeT<TMonad>, T> where TMonad : IMonad<TMonad>;

public sealed class MaybeT<TMonad> : IMonadPlus<MaybeT<TMonad>> where TMonad : IMonad<TMonad>
{
    public static IKind<MaybeT<TMonad>, V> Map<T, V>(IKind<MaybeT<TMonad>, T> f, Func<T, V> fun)
        => IMonad<MaybeT<TMonad>>.Map(f, fun);

    public static IKind<MaybeT<TMonad>, V> Apply<T, V>(IKind<MaybeT<TMonad>, T> applicative,
        IKind<MaybeT<TMonad>, Func<T, V>> fun) =>
        IMonad<MaybeT<TMonad>>.Apply(applicative, fun);

    public static IKind<MaybeT<TMonad>, V> Bind<T, V>(IKind<MaybeT<TMonad>, T> monad,
        Func<T, IKind<MaybeT<TMonad>, V>> fun)
    {
        var innerMonad = monad.To().InnerMonad;
        var res = TMonad.Bind(innerMonad, maybe => maybe switch
        {
            None<T> _ => TMonad.Pure(new None<V>().To()),
            Some<T> { Value: var t } => fun(t).To().InnerMonad,
            _ => throw new ArgumentException(nameof(monad))
        });
        return new MaybeT<TMonad, V>(res);
    }

    public static IKind<MaybeT<TMonad>, T> Pure<T>(T value) => new MaybeT<TMonad, T>(TMonad.Pure(Maybe.Pure(value).To()));

    public static IKind<MaybeT<TMonad>, T> Append<T, V>(IKind<MaybeT<TMonad>, T> a, IKind<MaybeT<TMonad>, T> b)
    {
        return new MaybeT<TMonad, T>(TMonad.Bind(a.To().InnerMonad, maybe => maybe switch
        {
            None<T> _ => b.To().InnerMonad,
            Some<T> { Value: var t } => TMonad.Pure(maybe),
            _ => throw new ArgumentException(nameof(a))
        }));
    }

    public static IKind<MaybeT<TMonad>, T> Empty<T>() => new MaybeT<TMonad, T>(TMonad.Pure(Maybe.Empty<T>().To()));
}

public static class MaybeTExt
{
    // public static Maybe<Z> SelectMany<T, V, Z>(this Maybe<T> maybe, Func<T, Maybe<V>> binder,
    //     Func<T, V, Z> projection)
    // {
    //     return Maybe.Bind(maybe, t => Maybe.Bind(binder(t), v => Maybe.Pure(projection(t, v)))).To();
    // }
    // public static Maybe<V> Select<T, V, TEnv>(this Maybe<T> maybe, Func<T, V> mapper)
    // {
    //     return Maybe.Map(maybe, mapper).To();
    // }
    public static MaybeT<TMonad, T> To<T, TMonad>(this IKind<MaybeT<TMonad>, T> kind) where TMonad : IMonad<TMonad>
    {
        return (MaybeT<TMonad, T>)kind;
    }
}