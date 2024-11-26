using FunctionalCSharp.New.Base;

namespace FunctionalCSharp.New.Monads;

public record ListT<TMonad, T>(IKind<TMonad, ListTStep> Next)
    : IKind<ListT<TMonad>, T> where TMonad : IMonad<TMonad>
{
    public static ListT<TMonad, T> Of(IKind<TMonad, ListTStep> next) => new(next);

    public static ListT<TMonad, T> FromStep(ListTStep listTStep) => Of(TMonad.Pure(listTStep));
}

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

public abstract class ListT<TMonad> : IMonadPlus<ListT<TMonad>> where TMonad : IMonad<TMonad>
{
    public static IKind<ListT<TMonad>, V> Map<T, V>(IKind<ListT<TMonad>, T> f, Func<T, V> fun)
    {
        throw new NotImplementedException();
    }

    public static IKind<ListT<TMonad>, V> Apply<T, V>(IKind<ListT<TMonad>, T> applicative,
        IKind<ListT<TMonad>, Func<T, V>> fun)
    {
        throw new NotImplementedException();
    }

    public static IKind<ListT<TMonad>, V> Bind<T, V>(IKind<ListT<TMonad>, T> monad,
        Func<T, IKind<ListT<TMonad>, V>> fun)
    {
        var next = monad.To().Next;
        var zz = TMonad.Bind<ListTStep, ListTStep>(next, step => step switch
        {
            Cons<TMonad, T> (var value, var rest) => Append(fun(value), Bind(rest, fun)).To().Next,
            Nil<TMonad> n => TMonad.Pure((ListTStep)n),
            _ => throw new ArgumentException(nameof(step))
        });
        return ListT<TMonad, V>.Of(zz);
    }

    public static IKind<ListT<TMonad>, T> Pure<T>(T value) =>
        ListT<TMonad, T>.FromStep(ListTStep.Cons(value,
            ListT<TMonad, T>.FromStep(ListTStep.Nil<TMonad>())));

    public static IKind<ListT<TMonad>, T> Append<T>(IKind<ListT<TMonad>, T> a, IKind<ListT<TMonad>, T> b)
    {
        var next = a.To().Next;
        return ListT<TMonad, T>.Of(TMonad.Bind(next, step => step switch
        {
            Cons<TMonad, T>(_, var rest) => Append(rest, b).To().Next,
            Nil<TMonad> _ => b.To().Next,
            _ => throw new ArgumentOutOfRangeException(nameof(step))
        }));
    }

    public static IKind<ListT<TMonad>, T> Empty<T>() => ListT<TMonad, T>.FromStep(ListTStep.Nil<TMonad>());
}

public static class ListTExtensions
{
    public static ListT<TMonad, T> To<TMonad, T>(this IKind<ListT<TMonad>, T> listT) where TMonad : IMonad<TMonad> =>
        (ListT<TMonad, T>)listT;
}