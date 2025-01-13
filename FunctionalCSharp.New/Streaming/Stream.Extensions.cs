using FunctionalCSharp.New.Base;

namespace FunctionalCSharp.New.Streaming;

public static class StreamExtensions
{
    public static Stream<TFunctor, TMonad, T> To<TFunctor, TMonad, T>(this IKind<Stream<TFunctor, TMonad>, T> kind)
        where TFunctor : IFunctor<TFunctor> where TMonad : IMonad<TMonad> => (Stream<TFunctor, TMonad, T>)kind;

    public static Stream<TFunctor, TMonad, Z> SelectMany<TFunctor, TMonad, T, V, Z>(
        this Stream<TFunctor, TMonad, T> stream,
        Func<T, Stream<TFunctor, TMonad, V>> binder,
        Func<T, V, Z> projection) where TMonad : IMonad<TMonad> where TFunctor : IFunctor<TFunctor>
    {
        return Stream<TFunctor, TMonad>
            .Bind(stream, t => binder(t).Select(v => projection(t, v))).To();
    }
    public static Stream<TFunctor, TMonad, V> Select<TFunctor, TMonad, T, V>(this Stream<TFunctor, TMonad, T> stream,
        Func<T, V> mapper)
        where TMonad : IMonad<TMonad> where TFunctor : IFunctor<TFunctor> =>
        Stream<TFunctor, TMonad>.Map(stream, mapper).To();
}