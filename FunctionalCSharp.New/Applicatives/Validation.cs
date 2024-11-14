﻿using FunctionalCSharp.New.Monads;

namespace FunctionalCSharp.New.Applicatives;

public abstract record Validation<T, TSemigroup, TError> : IKind<Validation<TSemigroup, TError>, T>
    where TSemigroup : ISemigroup<TSemigroup>;

public sealed record ValidationOk<T, TSemigroup, TError>(T Value)
    : Validation<T, TSemigroup, TError> where TSemigroup : ISemigroup<TSemigroup>
{
    public void Deconstruct(out T value)
    {
        value = Value;
    }
}

public sealed record ValidationFailed<T, TSemigroup, TError>(IKind<TSemigroup, TError> Errors)
    : Validation<T, TSemigroup, TError> where TSemigroup : ISemigroup<TSemigroup>
{
    public void Deconstruct(out IKind<TSemigroup, TError> errors)
    {
        errors = Errors;
    }
}

public abstract class Validation<TSemigroup, TError> : IApplicative<Validation<TSemigroup, TError>>
    where TSemigroup : ISemigroup<TSemigroup>
{
    public static IKind<Validation<TSemigroup, TError>, V> Map<T, V>(IKind<Validation<TSemigroup, TError>, T> f,
        Func<T, V> fun) =>
        Apply(f, Pure(fun));

    public static IKind<Validation<TSemigroup, TError>, V> Apply<T, V>(
        IKind<Validation<TSemigroup, TError>, T> applicative, IKind<Validation<TSemigroup, TError>, Func<T, V>> fun)
    {
        return (app: applicative.To(), func: fun.To()) switch
        {
            (ValidationOk<T, TSemigroup, TError>(var value), ValidationOk<Func<T, V>, TSemigroup, TError> (var func)) =>
                new
                    ValidationOk<V, TSemigroup, TError>(
                        func(value)),
            (ValidationOk<T, TSemigroup, TError> _, ValidationFailed<Func<T, V>, TSemigroup, TError>(var errors)) => new
                ValidationFailed<V, TSemigroup, TError>(
                    errors),
            (ValidationFailed<Func<T, V>, TSemigroup, TError>(var errors), ValidationOk<Func<T, V>, TSemigroup, TError>
                _) =>
                new ValidationFailed<V, TSemigroup, TError>(errors),
            (ValidationFailed<T, TSemigroup, TError>(var errors),
                ValidationFailed<Func<T, V>, TSemigroup, TError>(var errors2)) =>
                new ValidationFailed<V, TSemigroup, TError>(TSemigroup.Combine(errors, errors2)),
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    public static IKind<Validation<TSemigroup, TError>, Z> Lift2<T, V, Z>(Func<T, V, Z> operation,
        IKind<Validation<TSemigroup, TError>, T> app1,
        IKind<Validation<TSemigroup, TError>, V> app2) =>
        IApplicative<Validation<TSemigroup, TError>>.Lift2(operation, app1, app2);

    public static IKind<Validation<TSemigroup, TError>, T> Pure<T>(T value) =>
        new ValidationOk<T, TSemigroup, TError>(value);
}

public static class Validation
{
    public static Validation<T, TSemigroup, TError> To<T, TSemigroup, TError>(
        this IKind<Validation<TSemigroup, TError>, T> validation) where TSemigroup : ISemigroup<TSemigroup> =>
        (Validation<T, TSemigroup, TError>)validation;

    public static Validation<T, List, TError> Ok<T, TError>(T value) => new ValidationOk<T, List, TError>(value);

    public static Validation<T, List, TError> Failed<T, TError>(TError value) =>
        new ValidationFailed<T, List, TError>(List.Pure(value));
}