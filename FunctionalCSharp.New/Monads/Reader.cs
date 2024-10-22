using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FunctionalCSharp.New.Monads
{
    public record Reader<TEnv, T>(Func<TEnv, T> ReaderFunc) : IKind<Reader<TEnv>, T>
    {
        public T Run(TEnv env) => ReaderFunc(env);
        public Reader<TEnv, TEnv> Ask() => (Reader<TEnv, TEnv>)
            Reader<TEnv>.Bind(this, _ => new Reader<TEnv, TEnv>(env => env));
    }

    public class Reader<TEnv> : IMonad<Reader<TEnv>>
    {
        public static IKind<Reader<TEnv>, V> Map<T, V>(IKind<Reader<TEnv>, T> f, Func<T, V> fun)
            => IMonad<Reader<TEnv>>.Map(f, fun);

        public static IKind<Reader<TEnv>, V> Apply<T, V>(IKind<Reader<TEnv>, T> applicative, IKind<Reader<TEnv>, Func<T, V>> fun)
            => IMonad<Reader<TEnv>>.Apply(applicative, fun);

        public static IKind<Reader<TEnv>, V> Bind<T, V>(IKind<Reader<TEnv>, T> monad, Func<T, IKind<Reader<TEnv>, V>> fun)
        {
            var reader = (Reader<TEnv, T>)monad;
            return new Reader<TEnv, V>(env =>
            {
                var resT = reader.Run(env);
                var prodResult = (Reader<TEnv, V>)fun(resT);
                return prodResult.Run(env);
            });
        }

        public static Reader<TEnv, TEnv> Ask<T>(Reader<TEnv, T> reader) => reader.Ask();

        public static IKind<Reader<TEnv>, T> Pure<T>(T value)
        {
            return new Reader<TEnv, T>(_ => value);
        }
    }
}
