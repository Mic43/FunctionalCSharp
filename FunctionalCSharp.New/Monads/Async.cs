using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FunctionalCSharp.New.Monads;

public record Async<T> : IKind<Async, T>
{
    internal Func<T> AsyncJob { get; }

    internal Async(Func<T> AsyncJob)
    {
        this.AsyncJob = AsyncJob;
    }
    public static Async<T> FromResult(T result) => (Async<T>)Async.Pure(result);
    public T Run() => AsyncJob();
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
}

public static class AsyncExt
{
    public static Async<T> To<T>(this IKind<Async, T> kind)
    {
        return (Async<T>)kind;
    }
}