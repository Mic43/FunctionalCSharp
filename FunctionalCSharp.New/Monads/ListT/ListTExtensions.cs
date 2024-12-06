using FunctionalCSharp.New.Base;

namespace FunctionalCSharp.New.Monads.ListT;

public static class ListTExtensions
{
    public static ListT<TMonad, T> To<TMonad, T>(this IKind<ListT<TMonad>, T> listT) where TMonad : IMonad<TMonad> =>
        (ListT<TMonad, T>)listT;

    public static ListT<TMonad, Z> SelectMany<TMonad, T, V, Z>(this ListT<TMonad, T> listT,
        Func<T, ListT<TMonad, V>> binder,
        Func<T, V, Z> projection) where TMonad : IMonad<TMonad>
    {
        return ListT<TMonad>
            .Bind(listT, t => binder(t).Select(v => projection(t, v))).To();
    }

    public static ListT<TMonad, V> Select<TMonad, T, V>(this ListT<TMonad, T> listT, Func<T, V> mapper)
        where TMonad : IMonad<TMonad> =>
        ListT<TMonad>.Map(listT, mapper).To();

    public static ListT<TMonad, T> TakeWhile<TMonad, T>(this IKind<ListT<TMonad>, T> source,
        Predicate<T> predicate)
        where TMonad : IMonad<TMonad> =>
        ListT<TMonad>.TakeWhile(source, predicate);

    public static ListT<TMonad, T> Take<TMonad, T>(this IKind<ListT<TMonad>, T> source, int n)
        where TMonad : IMonad<TMonad> =>
        ListT<TMonad>.Take(source, n);

    public static ListT<TMonad, T> DropWhile<TMonad, T>(this IKind<ListT<TMonad>, T> source,
        Predicate<T> predicate)
        where TMonad : IMonad<TMonad> =>
        ListT<TMonad>.DropWhile(source, predicate);

    public static ListT<TMonad, T> Drop<TMonad, T>(this IKind<ListT<TMonad>, T> source, int n)
        where TMonad : IMonad<TMonad> =>
        ListT<TMonad>.Drop(source, n);

    public static ListT<TMonad, T> Where<T, TMonad>(this ListT<TMonad, T> source, Func<T, bool> predicate)
        where TMonad : IMonad<TMonad> => ListT<TMonad>.Where(source, predicate);

    public static IKind<TMonad, TResult> Fold<T, TResult, TMonad>(this IKind<ListT<TMonad>, T> source, TResult identity,
        Func<TResult, T, IKind<TMonad, TResult>> folder) where TMonad : IMonad<TMonad> =>
        ListT<TMonad>.Fold(source, identity, folder);

    public static IKind<TMonad, (List<T>, ListT<TMonad, T>)> SplitAt<T, TMonad>(this IKind<ListT<TMonad>, T> source,
        int index) where TMonad : IMonad<TMonad>
        => ListT<TMonad>.SplitAt(source, index);
}