using FunctionalCSharp.New.Base;

namespace FunctionalCSharp.New.Monads;

public sealed record MaybeT<TMonad, T> : IKind<MaybeT<TMonad>, T> where TMonad : IMonad<TMonad>
{
    internal MaybeT(IKind<TMonad, Maybe<T>> innerMonad)
    {
        RunMaybeT = innerMonad;
    }

    public static MaybeT<TMonad, T> Of(IKind<TMonad, Maybe<T>> innerMonad) => new(innerMonad);
    public IKind<TMonad, Maybe<T>> RunMaybeT { get; }
}

public abstract class MaybeT<TMonad> : IMonadPlus<MaybeT<TMonad>>, IMonadTransformer<MaybeT<TMonad>, TMonad>
    where TMonad : IMonad<TMonad>
{
    public static IKind<MaybeT<TMonad>, V> Map<T, V>(IKind<MaybeT<TMonad>, T> f, Func<T, V> fun)
        => IMonad<MaybeT<TMonad>>.Map(f, fun);

    public static IKind<MaybeT<TMonad>, V> Apply<T, V>(IKind<MaybeT<TMonad>, T> applicative,
        IKind<MaybeT<TMonad>, Func<T, V>> fun) =>
        IMonad<MaybeT<TMonad>>.Apply(applicative, fun);

    public static IKind<MaybeT<TMonad>, V> Bind<T, V>(IKind<MaybeT<TMonad>, T> monad,
        Func<T, IKind<MaybeT<TMonad>, V>> fun)
    {
        var innerMonad = monad.To().RunMaybeT;
        var res = TMonad.Bind(innerMonad, maybe => maybe switch
        {
            None<T> _ => TMonad.Pure(new None<V>().To()),
            Some<T> { Value: var t } => fun(t).To().RunMaybeT,
            _ => throw new ArgumentException(nameof(monad))
        });
        return new MaybeT<TMonad, V>(res);
    }

    public static IKind<MaybeT<TMonad>, T> Pure<T>(T value) =>
        new MaybeT<TMonad, T>(TMonad.Pure(Maybe.Pure(value).To()));

    public static IKind<MaybeT<TMonad>, T> Append<T>(IKind<MaybeT<TMonad>, T> a, IKind<MaybeT<TMonad>, T> b)
    {
        return new MaybeT<TMonad, T>(TMonad.Bind(a.To().RunMaybeT, maybe => maybe switch
        {
            None<T> _ => b.To().RunMaybeT,
            Some<T> { Value: var t } => TMonad.Pure(maybe),
            _ => throw new ArgumentException(nameof(a))
        }));
    }

    public static IKind<MaybeT<TMonad>, T> Empty<T>() => new MaybeT<TMonad, T>(TMonad.Pure(Maybe.Empty<T>().To()));

    public static IKind<MaybeT<TMonad>, T> Lift<T>(IKind<TMonad, T> monad) =>
        new MaybeT<TMonad, T>(TMonad.Map(monad, t => Maybe.Pure(t).To()));
}

public static class MaybeTExt
{
    public static MaybeT<TMonad, Z> SelectMany<T, V, Z, TMonad>(this MaybeT<TMonad, T> maybeT,
        Func<T, MaybeT<TMonad, V>> binder,
        Func<T, V, Z> projection) where TMonad : IMonad<TMonad>
    {
        return MaybeT<TMonad>.Bind(maybeT, t => binder(t).Select(v => projection(t, v))).To();
    }

    public static MaybeT<TMonad, V> Select<T, V, TMonad>(this MaybeT<TMonad, T> maybeT, Func<T, V> mapper)
        where TMonad : IMonad<TMonad> =>
        MaybeT<TMonad>.Map(maybeT, mapper).To();

    public static MaybeT<TMonad, T> To<T, TMonad>(this IKind<MaybeT<TMonad>, T> kind) where TMonad : IMonad<TMonad> =>
        (MaybeT<TMonad, T>)kind;
}