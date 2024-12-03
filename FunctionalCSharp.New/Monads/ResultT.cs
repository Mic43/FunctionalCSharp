using FunctionalCSharp.New.Base;

namespace FunctionalCSharp.New.Monads;

public record ResultT<TError, TMonad, T>(IKind<TMonad, IKind<Result<TError>, T>> InnerMonad)
    : IKind<ResultT<TError, TMonad>, T> where TMonad : IMonad<TMonad>;

public abstract class ResultT<TError, TMonad> : IMonad<ResultT<TError, TMonad>>,
    IMonadTransformer<ResultT<TError, TMonad>, TMonad> where TMonad : IMonad<TMonad>
{
    public static IKind<ResultT<TError, TMonad>, V> Map<T, V>(IKind<ResultT<TError, TMonad>, T> f, Func<T, V> fun) =>
        IMonad<ResultT<TError, TMonad>>.Map(f, fun);

    public static IKind<ResultT<TError, TMonad>, V> Apply<T, V>(IKind<ResultT<TError, TMonad>, T> applicative,
        IKind<ResultT<TError, TMonad>, Func<T, V>> fun) =>
        IMonad<ResultT<TError, TMonad>>.Apply(applicative, fun);

    public static IKind<ResultT<TError, TMonad>, V> Bind<T, V>(IKind<ResultT<TError, TMonad>, T> monad,
        Func<T, IKind<ResultT<TError, TMonad>, V>> fun)
    {
        var innerMonad = monad.To().InnerMonad;
        return new ResultT<TError, TMonad, V>(
            TMonad.Bind(innerMonad, result =>
                result.To().Either(
                    t => fun(t).To().InnerMonad,
                    error => TMonad.Pure<>(Result<V, TError>.Error(error)))));
    }

    public static IKind<ResultT<TError, TMonad>, T> Pure<T>(T value) =>
        new ResultT<TError, TMonad, T>(TMonad.Map(TMonad.Pure(value), Result<TError>.Pure));

    public static IKind<ResultT<TError, TMonad>, T> Lift<T>(IKind<TMonad, T> monad) =>
        new ResultT<TError, TMonad, T>(TMonad.Map(monad, Result<TError>.Pure));
}

public static class ResultTExt
{
    public static ResultT<TError, TMonad, Z> SelectMany<T, V, Z, TMonad, TError>(
        this ResultT<TError, TMonad, T> resultT,
        Func<T, ResultT<TError, TMonad, V>> binder,
        Func<T, V, Z> projection) where TMonad : IMonad<TMonad>
    {
        return ResultT<TError, TMonad>
            .Bind(resultT,
                t => binder(t).Select(v => projection(t, v))).To();
    }

    public static ResultT<TError, TMonad, V> Select<T, V, TMonad, TError>(this ResultT<TError, TMonad, T> resultT,
        Func<T, V> mapper)
        where TMonad : IMonad<TMonad>
    {
        return ResultT<TError, TMonad>.Map(resultT, mapper).To();
    }

    public static ResultT<TError, TMonad, T> To<TError, TMonad, T>(this IKind<ResultT<TError, TMonad>, T> kind)
        where TMonad : IMonad<TMonad>
    {
        return (ResultT<TError, TMonad, T>)kind;
    }
}