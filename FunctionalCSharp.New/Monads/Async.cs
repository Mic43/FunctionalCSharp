using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FunctionalCSharp.New.Base;

namespace FunctionalCSharp.New.Monads;

public record Async<T> : IKind<Async, T>
{
    internal Func<T> AsyncJob { get; }

    internal Async(Func<T> AsyncJob)
    {
        this.AsyncJob = AsyncJob;
    }

    public static Async<T> FromResult(T result) => (Async<T>)Async.Pure(result);

    public static Async<T> FromTask(Task<T> task)
    {
        return new Async<T>(() => task.Result);
    }

    public T Run() => AsyncJob();

    public Async<Result<T, Exception>> Catch()
    {
        return new Async<Result<T, Exception>>(() =>
        {
            try
            {
                return Result<T, Exception>.Ok(Run());
            }
            catch (Exception e)
            {
                return Result<T, Exception>.Error(e);
            }
        });
    }
}

public abstract class Async : IMonad<Async>
{
    public static IKind<Async, V> Apply<T, V>(IKind<Async, T> applicative, IKind<Async, Func<T, V>> fun)
        => IMonad<Async>.Apply(applicative, fun);

    public static IKind<Async, Z> Lift2<T, V, Z>(Func<T, V, Z> operation, IKind<Async, T> app1,
        IKind<Async, V> app2) =>
        IMonad<Async>.Lift2(operation, app1, app2);

    public static IKind<Async, T> Join<T>(IKind<Async, IKind<Async, T>> monad) => IMonad<Async>.Join(monad);

    public static IKind<Async, V> Bind<T, V>(IKind<Async, T> monad, Func<T, IKind<Async, V>> fun)
        => new Async<V>(() =>
        {
            var res = ((Async<T>)monad).Run();
            return ((Async<V>)fun(res)).Run();
        });

    public static IKind<Async, V> Map<T, V>(IKind<Async, T> f, Func<T, V> fun) => IMonad<Async>.Map(f, fun);

    public static IKind<Async, T> Pure<T>(T value) => new Async<T>(() => value);
    public static T Run<T>(IKind<Async, T> async) => ((Async<T>)async).Run();

    public static Async<Result<T, Exception>> Catch<T>(Async<T> async) => async.Catch();
}

public static class AsyncExt
{
    public static Async<Z> SelectMany<T, V, Z>(this Async<T> async, Func<T, Async<V>> binder,
        Func<T, V, Z> projection)
    {
        return Async.Bind(async,
            t => binder(t).Select(v => projection(t, v))).To();
    }

    public static Async<V> Select<T, V>(this Async<T> async, Func<T, V> mapper) => Async.Map(async, mapper).To();

    public static Async<T> To<T>(this IKind<Async, T> kind)
    {
        return (Async<T>)kind;
    }

    public static Async<T> ToAsync<T>(this Task<T> task)
    {
        return Async<T>.FromTask(task);
    }
}