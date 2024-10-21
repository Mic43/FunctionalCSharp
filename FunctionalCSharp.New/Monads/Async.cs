using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FunctionalCSharp.New.Monads
{
    public record Async<T>(Func<T> AsyncJob) : IKind<Async, T>
    {
        public static Async<T> FromResult(T result) => (Async<T>)Async.Pure(result);
        public T Run() => AsyncJob();
    }
    public sealed class Async : IMonad<Async>
    {
        public static IKind<Async, V> Apply<T, V>(IKind<Async, T> applicative, IKind<Async, Func<T, V>> fun)
            => IMonad<Async>.Apply(applicative, fun);

        public static IKind<Async, V> Bind<T, V>(IKind<Async, T> monad, Func<T, IKind<Async, V>> fun)
            => new Async<V>(() =>
                {
                    var res = ((Async<T>)monad).Run();
                    return ((Async<V>)fun(res)).Run();
                });

        public static IKind<Async, V> Map<T, V>(IKind<Async, T> f, Func<T, V> fun) => IMonad<Async>.Map(f, fun);

        public static IKind<Async, T> Pure<T>(T value) => new Async<T>(() => value);
        public static T RunAsync<T>(IKind<Async, T> async) => ((Async<T>)async).Run();
    }
}
