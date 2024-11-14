﻿namespace FunctionalCSharp.New.Monads;

public abstract record Result<T, TError> : IKind<Result<TError>, T>;

public sealed record Ok<T, TError>(T ResultValue) : Result<T, TError>
{
    public void Deconstruct(out T resultValue)
    {
        resultValue = ResultValue;
    }
}

public sealed record Error<T, TError>(TError ErrorValue) : Result<T, TError>
{
    public void Deconstruct(out TError errorValue)
    {
        errorValue = ErrorValue;
    }
}

public abstract class Result<TError> : IMonad<Result<TError>>
{
    public static IKind<Result<TError>, V> Map<T, V>(IKind<Result<TError>, T> f, Func<T, V> fun)
        => IMonad<Result<TError>>.Map(f, fun);

    public static IKind<Result<TError>, V> Apply<T, V>(IKind<Result<TError>, T> applicative,
        IKind<Result<TError>, Func<T, V>> fun)
        => IMonad<Result<TError>>.Apply(applicative, fun);

    public static IKind<Result<TError>, Z> Lift2<T, V, Z>(Func<T, V, Z> operation, IKind<Result<TError>, T> app1,
        IKind<Result<TError>, V> app2) =>
        IMonad<Result<TError>>.Lift2(operation, app1, app2);

    public static IKind<Result<TError>, V> Bind<T, V>(IKind<Result<TError>, T> monad,
        Func<T, IKind<Result<TError>, V>> fun)
    {
        return monad.To() switch
        {
            Ok<T, TError> (var value) => fun(value),
            Error<T, TError> (var error) => new Error<V, TError>(error),
            _ => throw new ArgumentOutOfRangeException(nameof(monad))
        };
    }

    public static IKind<Result<TError>, T> Join<T>(IKind<Result<TError>, IKind<Result<TError>, T>> monad)
        => IMonad<Result<TError>>.Join(monad);

    public static IKind<Result<TError>, T> Pure<T>(T value) => new Ok<T, TError>(value);
}

public static class ResultExt
{
    public static Result<Z, TError> SelectMany<T, V, Z, TError>(this Result<T, TError> result,
        Func<T, Result<V, TError>> binder,
        Func<T, V, Z> projection) =>
        Result<TError>
            .Bind(result, t => binder(t).Select(v => projection(t, v))).To();

    public static Result<V, TError> Select<T, V, TError>(this Result<T, TError> result, Func<T, V> mapper) =>
        Result<TError>.Map(result, mapper).To();

    public static Result<T, TError> To<T, TError>(this IKind<Result<TError>, T> kind) => (Result<T, TError>)kind;
}