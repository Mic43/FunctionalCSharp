using FunctionalCSharp.New.Base;

namespace FunctionalCSharp.New.Streaming;

public abstract class Stream<TFunctor, TMonad> : IMonad<Stream<TFunctor, TMonad>>,
    IMonadTransformer<Stream<TFunctor, TMonad>, TMonad>
    where TFunctor : IFunctor<TFunctor> where TMonad : IMonad<TMonad>
{
    public static IKind<Stream<TFunctor, TMonad>, V> Map<T, V>(IKind<Stream<TFunctor, TMonad>, T> f, Func<T, V> fun) =>
        IMonad<Stream<TFunctor, TMonad>>.Map(f, fun);

    public static IKind<Stream<TFunctor, TMonad>, V> Apply<T, V>(IKind<Stream<TFunctor, TMonad>, T> applicative,
        IKind<Stream<TFunctor, TMonad>, Func<T, V>> fun) =>
        IMonad<Stream<TFunctor, TMonad>>.Apply(applicative, fun);

    public static IKind<Stream<TFunctor, TMonad>, V> Bind<T, V>(IKind<Stream<TFunctor, TMonad>, T> monad,
        Func<T, IKind<Stream<TFunctor, TMonad>, V>> fun)
    {
        return monad.To() switch
        {
            Effect<TFunctor, TMonad, T>(var eff) => new Effect<TFunctor, TMonad, V>
            (from s in eff
                select Bind<T, V>(s, fun).To()),
            Return<TFunctor, TMonad, T>(var value) => fun(value),
            Step<TFunctor, TMonad, T>(var functor) =>
                new Step<TFunctor, TMonad, V>(TFunctor.Map(functor, stream => Bind(stream, fun).To())),
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    public static IKind<Stream<TFunctor, TMonad>, T> Join<T>(
        IKind<Stream<TFunctor, TMonad>, IKind<Stream<TFunctor, TMonad>, T>> monad)
        => IMonad<Stream<TFunctor, TMonad>>.Join(monad);
    
    public static IKind<Stream<TFunctor, TMonad>, T> Pure<T>(T value) => new Return<TFunctor, TMonad, T>(value);

    public static IKind<Stream<TFunctor, TMonad>, T> Lift<T>(IKind<TMonad, T> monad) =>
        new Effect<TFunctor, TMonad, T>(
            from t in monad
            select Pure(t).To());
}