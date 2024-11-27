using FunctionalCSharp.New.Base;

namespace FunctionalCSharp.New.Monads.ListT;

public abstract record ListTStep
{
    public static ListTStep Nil<TMonad>() where TMonad : IMonad<TMonad> => new Nil<TMonad>();

    public static ListTStep Cons<TMonad, T>(T value, ListT<TMonad, T> rest) where TMonad : IMonad<TMonad>
        => new Cons<TMonad, T>(value, rest);
}

public record Nil<TMonad> : ListTStep where TMonad : IMonad<TMonad>;

public record Cons<TMonad, T>(T Value, ListT<TMonad, T> Rest) : ListTStep where TMonad : IMonad<TMonad>
{
    public void Deconstruct(out T value, out ListT<TMonad, T> rest)
    {
        value = Value;
        rest = Rest;
    }
}
