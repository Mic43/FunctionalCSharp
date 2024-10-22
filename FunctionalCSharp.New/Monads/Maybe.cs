namespace FunctionalCSharp.New.Monads
{
    public abstract record Maybe<T> : IKind<Maybe, T>;
    public sealed record None<T> : Maybe<T>;
    public sealed record Some<T>(T Value) : Maybe<T>;

    public sealed class Maybe : IMonad<Maybe>
    {
        public static IKind<Maybe, V> Map<T, V>(IKind<Maybe, T> f, Func<T, V> fun) => IMonad<Maybe>.Map(f, fun);
        public static IKind<Maybe, V> Apply<T, V>(IKind<Maybe, T> applicative, IKind<Maybe, Func<T, V>> fun) => IMonad<Maybe>.Apply(applicative, fun);

        public static IKind<Maybe, V> Bind<T, V>(IKind<Maybe, T> monad, Func<T, IKind<Maybe, V>> fun) => (Maybe<T>)monad switch
        {
            None<T> _ => new None<V>(),
            Some<T> { Value: var v } => fun(v),
            _ => throw new ArgumentException(nameof(monad))
        };

        public static IKind<Maybe, T> Join<T>(IKind<Maybe, IKind<Maybe, T>> monad) => IMonad<Maybe>.Join(monad);

        public static IKind<Maybe, T> Pure<T>(T value) => new Some<T>(value);
    }
}
