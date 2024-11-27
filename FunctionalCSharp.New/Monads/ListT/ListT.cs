using FunctionalCSharp.New.Base;

namespace FunctionalCSharp.New.Monads.ListT;

public record ListT<TMonad, T> : IKind<ListT<TMonad>, T> where TMonad : IMonad<TMonad>
{
    internal ListT(IKind<TMonad, ListTStep> Next) => this.Next = Next;

    public static ListT<TMonad, T> Of(IKind<TMonad, ListTStep> next) => new(next);
    public static ListT<TMonad, T> FromStep(ListTStep listTStep) => Of(TMonad.Pure(listTStep));
    public IKind<TMonad, ListTStep> Next { get; }

    public IKind<TMonad, Unit> RunListT() =>
        TMonad.Bind(Next, step => step switch
        {
            Cons<TMonad, T>(_, var rest) => rest.RunListT(),
            Nil<TMonad> _ => TMonad.Pure(Unit.Instance()),
            _ => throw new ArgumentOutOfRangeException(nameof(step))
        });
          
    public static ListT<TMonad, T> operator +(ListT<TMonad, T> left, ListT<TMonad, T> right)
    {
        return ListT<TMonad>.Append(left, right).To();
    }
}

public abstract class ListT<TMonad> : IMonadPlus<ListT<TMonad>> where TMonad : IMonad<TMonad>
{
    public static IKind<ListT<TMonad>, V> Map<T, V>(IKind<ListT<TMonad>, T> f, Func<T, V> fun) =>
        IMonad<ListT<TMonad>>.Map(f, fun);

    public static IKind<ListT<TMonad>, V> Apply<T, V>(IKind<ListT<TMonad>, T> applicative,
        IKind<ListT<TMonad>, Func<T, V>> fun) =>
        IMonad<ListT<TMonad>>.Apply(applicative, fun);

    public static IKind<ListT<TMonad>, V> Bind<T, V>(IKind<ListT<TMonad>, T> monad,
        Func<T, IKind<ListT<TMonad>, V>> fun)
    {
        var next = monad.To().Next;
        return ListT<TMonad, V>.Of(TMonad.Bind<ListTStep, ListTStep>(next, step => step switch
        {
            Cons<TMonad, T> (var value, var rest) => Append(fun(value), Bind(rest, fun)).To().Next,
            Nil<TMonad> n => TMonad.Pure((ListTStep)n),
            _ => throw new ArgumentException(nameof(step))
        }));
    }

    /// <summary>
    /// Flattens this listT
    /// </summary>
    /// <param name="monad"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static IKind<ListT<TMonad>, T> Join<T>(IKind<ListT<TMonad>, IKind<ListT<TMonad>, T>> monad) =>
        IMonad<ListT<TMonad>>.Join(monad);

    public static IKind<ListT<TMonad>, T> Pure<T>(T value) =>
        ListT<TMonad, T>.FromStep(ListTStep.Cons(value,
            ListT<TMonad, T>.FromStep(ListTStep.Nil<TMonad>())));

    public static IKind<ListT<TMonad>, T> Append<T>(IKind<ListT<TMonad>, T> a, IKind<ListT<TMonad>, T> b)
    {
        var next = a.To().Next;
        return ListT<TMonad, T>.Of(TMonad.Bind(next, step => step switch
        {
            Cons<TMonad, T>(var value, var rest) cons =>
                TMonad.Pure<ListTStep>(cons with { Rest = Append(rest, b).To() }),
            Nil<TMonad> _ => b.To().Next,
            _ => throw new ArgumentOutOfRangeException(nameof(step))
        }));
    }

    public static IKind<ListT<TMonad>, T> Empty<T>() => ListT<TMonad, T>.FromStep(ListTStep.Nil<TMonad>());

    public static IKind<ListT<TMonad>, T> Lift<T>(IKind<TMonad, T> monad) =>
        ListT<TMonad, T>.Of(TMonad.Map(monad,
            t => ListTStep.Cons(t, ListT<TMonad, T>.FromStep(ListTStep.Nil<TMonad>()))));
    
}