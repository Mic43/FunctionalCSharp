using FunctionalCSharp.New.Base;

namespace FunctionalCSharp.New.Streaming;

public static class StreamExtensions
{
    public static Stream<TFunctor, TMonad, T> To<TFunctor, TMonad, T>(this IKind<Stream<TFunctor, TMonad>, T> kind)
        where TFunctor : IFunctor<TFunctor> where TMonad : IMonad<TMonad> => (Stream<TFunctor, TMonad, T>)kind;
}