using FunctionalCSharp.New.Base;

namespace FunctionalCSharp.New.Streaming;

public abstract record Stream<TFunctor, TMonad, T> : IKind<Stream<TFunctor, TMonad>, T>
    where TFunctor : IFunctor<TFunctor> where TMonad : IMonad<TMonad>;

public record Return<TFunctor, TMonad, T>(T Value)
    : Stream<TFunctor, TMonad, T> where TFunctor : IFunctor<TFunctor> where TMonad : IMonad<TMonad>
{
    public void Deconstruct(out T value) => value = Value;
}

public record Step<TFunctor, TMonad, T>(IKind<TFunctor, Stream<TFunctor, TMonad, T>> Value)
    : Stream<TFunctor, TMonad, T> where TFunctor : IFunctor<TFunctor> where TMonad : IMonad<TMonad>
{
    public void Deconstruct(out IKind<TFunctor, Stream<TFunctor, TMonad, T>> value) => value = Value;
}

public record Effect<TFunctor, TMonad, T>(IKind<TMonad, Stream<TFunctor, TMonad, T>> Value)
    : Stream<TFunctor, TMonad, T> where TFunctor : IFunctor<TFunctor> where TMonad : IMonad<TMonad>
{
    public void Deconstruct(out IKind<TMonad, Stream<TFunctor, TMonad, T>> value) => value = Value;
}