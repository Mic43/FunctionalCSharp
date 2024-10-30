namespace FunctionalCSharp.New.Monads;

public abstract record Maybe<T> : IKind<Maybe, T>
{
    public static Maybe<T> None() => new None<T>();
}

public sealed record None<T> : Maybe<T>;

public sealed record Some<T>(T Value) : Maybe<T>
{
    public static Some<T> Of(T value) => new(value);
}

public sealed class Maybe : IMonadPlus<Maybe>
{
    public static IKind<Maybe, V> Map<T, V>(IKind<Maybe, T> f, Func<T, V> fun) => IMonad<Maybe>.Map(f, fun);

    public static IKind<Maybe, V> Apply<T, V>(IKind<Maybe, T> applicative, IKind<Maybe, Func<T, V>> fun) =>
        IMonad<Maybe>.Apply(applicative, fun);

    public static IKind<Maybe, Z> Lift2<T, V, Z>(Func<T, V, Z> operation, IKind<Maybe, T> app1,
        IKind<Maybe, V> app2) =>
        IMonad<Maybe>.Lift2(operation, app1, app2);

    public static IKind<Maybe, V> Bind<T, V>(IKind<Maybe, T> monad, Func<T, IKind<Maybe, V>> fun) => monad.To() switch
    {
        None<T> _ => new None<V>(),
        Some<T> { Value: var v } => fun(v),
        _ => throw new ArgumentException(nameof(monad))
    };

    public static IKind<Maybe, T> Join<T>(IKind<Maybe, IKind<Maybe, T>> monad) => IMonad<Maybe>.Join(monad);

    public static IKind<Maybe, T> Pure<T>(T value) => new Some<T>(value);

    public static IKind<Maybe, T> Append<T, V>(IKind<Maybe, T> a, IKind<Maybe, T> b) =>
        (a.To(), b.To()) switch
        {
            (None<T> _, None<T> _) => new None<T>(),
            (None<T> _, Some<T> v) => v,
            (Some<T> v, None<T> _) => v,
            (Some<T> v1, Some<T> _) => v1,
            _ => throw new ArgumentException(nameof(a))
        };

    public static IKind<Maybe, T> Empty<T>() => new None<T>();
}

/// <summary>
/// Some helper functions for query comprehensions to work
/// </summary>
public static class MaybeExt
{
    public static Maybe<Z> SelectMany<T, V, Z>(this Maybe<T> maybe, Func<T, Maybe<V>> binder,
        Func<T, V, Z> projection)
    {
        return Maybe.Bind(maybe, t => Maybe.Bind(binder(t), v => Maybe.Pure(projection(t, v)))).To();
    }

    public static Maybe<V> Select<T, V, TEnv>(this Maybe<T> maybe, Func<T, V> mapper)
    {
        return Maybe.Map(maybe, mapper).To();
    }

    public static Maybe<T> To<T>(this IKind<Maybe, T> kind)
    {
        return (Maybe<T>)kind;
    }
}