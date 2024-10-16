namespace FunctionalCSharp.New
{
    public abstract record Maybe<T> : IKind<Maybe, T>;
    public record None<T> : Maybe<T>;
    public record Some<T>(T Value) : Maybe<T>;

    public class Maybe : IMonad<Maybe>
    {
        public static IKind<Maybe, V> Map<T, V>(IKind<Maybe, T> f, Func<T, V> fun)
        {
            return IMonad<Maybe>.Map(f, fun);
        }
        public static IKind<Maybe, V> Apply<T, V>(IKind<Maybe, T> applicative, IKind<Maybe, Func<T, V>> fun)
        {
            return IMonad<Maybe>.Apply(applicative, fun);
        }

        public static IKind<Maybe, V> Bind<T, V>(IKind<Maybe, T> monad, Func<T, IKind<Maybe, V>> fun)
        {
            return (Maybe<T>)monad switch
            {
                None<T> n => new None<V>(),
                Some<T> { Value: var v } => fun(v)
            };
        }
        public static IKind<Maybe, T> Join<T>(IKind<Maybe, IKind<Maybe, T>> monad)
        {
            return IMonad<Maybe>.Join(monad);
        }

        public static IKind<Maybe, T> Pure<T>(T value)
        {
            return new Some<T>(value);
        }
    }
}
