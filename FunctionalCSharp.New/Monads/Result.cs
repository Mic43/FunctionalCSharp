﻿using System.Reflection.Metadata.Ecma335;

namespace FunctionalCSharp.New.Monads;

public abstract record Result<T, TError> : IKind<Result<TError>, T>;
public sealed record Ok<T, TError>(T ResultValue) : Result<T, TError>;
public sealed record Error<T, TError>(TError ErrorValue) : Result<T, TError>;


public sealed class Result<TError> : IMonad<Result<TError>>
{
    public static IKind<Result<TError>, V> Map<T, V>(IKind<Result<TError>, T> f, Func<T, V> fun)
        => IMonad<Result<TError>>.Map(f, fun);

    public static IKind<Result<TError>, V> Apply<T, V>(IKind<Result<TError>, T> applicative, IKind<Result<TError>, Func<T, V>> fun)
        => IMonad<Result<TError>>.Apply(applicative, fun);

    public static IKind<Result<TError>, V> Bind<T, V>(IKind<Result<TError>, T> monad, Func<T, IKind<Result<TError>, V>> fun)
    {
        Result<T, TError> m = (Result<T, TError>)monad;

        return m switch
        {
            Ok<T, TError> ok => fun(ok.ResultValue),
            Error<T, TError> error => new Error<V, TError>(error.ErrorValue),
            _ => throw new ArgumentOutOfRangeException(nameof(monad))
        };
    }

    public static IKind<Result<TError>, T> Pure<T>(T value)
    {
        return new Ok<T, TError>(value);
    }
}
