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
    
    public static IKind<ListT<TMonad>, T> TakeWhile<TMonad, T>(this IKind<ListT<TMonad>, T> source, Predicate<T> predicate)
        where TMonad : IMonad<TMonad> =>
        ListT<TMonad>.TakeWhile(source, predicate);
    
    public static IKind<ListT<TMonad>, T> Take<TMonad, T>(this IKind<ListT<TMonad>, T> source, int n)
        where TMonad : IMonad<TMonad> =>
        ListT<TMonad>.Take(source, n);
}