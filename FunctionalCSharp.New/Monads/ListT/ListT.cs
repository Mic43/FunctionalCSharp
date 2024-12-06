using FunctionalCSharp.New.Base;

namespace FunctionalCSharp.New.Monads.ListT;

public record ListT<TMonad, T> : IKind<ListT<TMonad>, T> where TMonad : IMonad<TMonad>
{
    internal ListT(IKind<TMonad, ListTStep> Next) => this.Next = Next;

    public static ListT<TMonad, T> Of(IKind<TMonad, ListTStep> next) => new(next);
    public static ListT<TMonad, T> FromStep(ListTStep listTStep) => Of(TMonad.Pure(listTStep));
    public IKind<TMonad, ListTStep> Next { get; }

    public IKind<TMonad, Unit> RunListT =>
        TMonad.Bind(Next, step => step switch
        {
            Cons<TMonad, T>(_, var rest) => rest.RunListT,
            Nil<TMonad> _ => TMonad.Pure(Unit.Instance()),
            _ => throw new ArgumentOutOfRangeException(nameof(step))
        });

    /// <summary>
    /// Concatenates two lists using Append
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    /// <returns>Concatenated list</returns>
    public static ListT<TMonad, T> operator +(ListT<TMonad, T> left, ListT<TMonad, T> right)
    {
        return ListT<TMonad>.Append(left, right).To();
    }
}

public abstract class ListT<TMonad> : IMonadPlus<ListT<TMonad>>, IMonadTransformer<ListT<TMonad>, TMonad>
    where TMonad : IMonad<TMonad>
{
    private static IKind<TMonad, ListTStep> Nil => TMonad.Pure(ListTStep.Nil<TMonad>());

    /// <summary>
    /// Produces infinite list of provided value
    /// </summary>
    /// <param name="value">Value to use</param>
    /// <typeparam name="T">type of elements in list</typeparam>
    /// <returns>infinite list</returns>
    public static ListT<TMonad, T> Repeat<T>(T value)
    {
        return
            from v in Pure(value).To()
            from res in ListT<TMonad, T>.FromStep(ListTStep.Cons(v, Repeat(v)))
            select res;
    }

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
            Nil<TMonad> _ => Nil,
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
        ListT<TMonad, T>.FromStep(ListTStep.Cons(value, ListT<TMonad, T>.Of(Nil)));

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

    public static IKind<ListT<TMonad>, T> Empty<T>() => ListT<TMonad, T>.Of(Nil);

    public static IKind<ListT<TMonad>, T> Lift<T>(IKind<TMonad, T> monad) =>
        ListT<TMonad, T>.Of(TMonad.Map(monad,
            t => ListTStep.Cons(t, ListT<TMonad, T>.Of(Nil))));

    public static ListT<TMonad, T> TakeWhile<T>(IKind<ListT<TMonad>, T> source, Predicate<T> predicate)
    {
        var next = source.To().Next;
        return ListT<TMonad, T>.Of(TMonad.Bind(next, step => step switch
        {
            Cons<TMonad, T>(var value, var rest) cons => predicate(value)
                ? TMonad.Pure<ListTStep>(cons with { Rest = TakeWhile(rest, predicate).To() })
                : Nil,
            Nil<TMonad> _ => Nil,
            _ => throw new ArgumentOutOfRangeException(nameof(step))
        }));
    }

    public static ListT<TMonad, T> Take<T>(IKind<ListT<TMonad>, T> source, int n)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(n);

        var next = source.To().Next;
        return ListT<TMonad, T>.Of(TMonad.Bind(next, step => step switch
        {
            Cons<TMonad, T>(_, var rest) cons => n > 0
                ? TMonad.Pure<ListTStep>(cons with { Rest = Take(rest, n - 1).To() })
                : Nil,
            Nil<TMonad> _ => Nil,
            _ => throw new ArgumentOutOfRangeException(nameof(step))
        }));
    }

    public static ListT<TMonad, T> DropWhile<T>(IKind<ListT<TMonad>, T> source, Predicate<T> predicate)
    {
        var next = source.To().Next;
        return ListT<TMonad, T>.Of(TMonad.Bind(next, step => step switch
        {
            Cons<TMonad, T>(var value, var rest) cons => predicate(value)
                ? DropWhile(rest, predicate).To().Next
                : TMonad.Pure<ListTStep>(cons),
            Nil<TMonad> _ => Nil,
            _ => throw new ArgumentOutOfRangeException(nameof(step))
        }));
    }

    public static ListT<TMonad, T> Drop<T>(IKind<ListT<TMonad>, T> source, int n)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(n);

        var next = source.To().Next;
        return ListT<TMonad, T>.Of(TMonad.Bind(next, step => step switch
        {
            Cons<TMonad, T>(_, var rest) cons => n > 0
                ? Drop(rest, n - 1).To().Next
                : TMonad.Pure<ListTStep>(cons),
            Nil<TMonad> _ => Nil,
            _ => throw new ArgumentOutOfRangeException(nameof(step))
        }));
    }

    public static ListT<TMonad, T> Where<T>(IKind<ListT<TMonad>, T> source, Func<T, bool> predicate)
    {
        var next = source.To().Next;
        return ListT<TMonad, T>.Of(TMonad.Bind(next, step => step switch
        {
            Cons<TMonad, T>(var value, var rest) cons => predicate(value)
                ? TMonad.Pure<ListTStep>(cons with { Rest = Where(rest, predicate).To() })
                : Where(rest, predicate).To().Next,
            Nil<TMonad> _ => Nil,
            _ => throw new ArgumentOutOfRangeException(nameof(step))
        }));
    }

    public static IKind<TMonad, (List<T>, ListT<TMonad, T>)> SplitAt<T>(IKind<ListT<TMonad>, T> source, int index)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(index);

        var next = source.To().Next;
        return TMonad.Bind(next, step => step switch
        {
            Cons<TMonad, T> (var value, var rest) => index > 0
                ? TMonad.Map(SplitAt(rest, index - 1),
                    tuple => tuple with { Item1 = tuple.Item1.Prepend(value) })
                : TMonad.Pure((List.Pure(value).To(), rest)),
            Nil<TMonad> _ => TMonad.Pure((List.Empty<T>().To(), Empty<T>().To())),
            _ => throw new ArgumentException("step")
        });
    }

    public static IKind<TMonad, TResult> Fold<T, TResult>(IKind<ListT<TMonad>, T> source, TResult identity,
        Func<TResult, T, IKind<TMonad, TResult>> folder)
    {
        IKind<TMonad, TResult> FoldInner(IKind<ListT<TMonad>, T> source, IKind<TMonad, TResult> acc)
        {
            var next = source.To().Next;
            return TMonad.Bind(next, step => step switch
            {
                Cons<TMonad, T>(var value, var rest) =>
                    FoldInner(rest, TMonad.Bind(acc, res => folder(res, value))),
                Nil<TMonad> _ => acc,
                _ => throw new ArgumentOutOfRangeException(nameof(step))
            });
        }
        return FoldInner(source, TMonad.Pure(identity));
    }
}