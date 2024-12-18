using System.Numerics;
using System.Runtime.CompilerServices;
using FunctionalCSharp.New.Base;

namespace FunctionalCSharp.New.Monads;

public record List<T>(IEnumerable<T> SourceList) : IKind<List, T>
{
    public List() : this(Enumerable.Empty<T>())
    {
    }

    public IEnumerable<T> SourceList { get; } = SourceList;
}

public abstract class List : IMonadPlus<List>, ITraversable<List>, IMonoid<List>
{
    public static IKind<List, V> Map<T, V>(IKind<List, T> f, Func<T, V> fun) => IMonad<List>.Map(f, fun);

    public static IKind<List, V> Apply<T, V>(IKind<List, T> applicative, IKind<List, Func<T, V>> fun) =>
        IMonad<List>.Apply(applicative, fun);

    public static IKind<List, V> Bind<T, V>(IKind<List, T> monad, Func<T, IKind<List, V>> fun)
    {
        return new List<V>(monad.To().SourceList.SelectMany(t => fun(t).To().SourceList));
    }

    public static IKind<List, T> Pure<T>(T value) => new List<T>(Enumerable.Repeat(value, 1));

    public static List<T> Prepend<T>(T item, IKind<List, T> list) =>
        Append(Pure(item), list).To();

    public static IKind<List, T> Append<T>(IKind<List, T> a, IKind<List, T> b) =>
        new List<T>(a.To().SourceList.Concat(b.To().SourceList));

    public static IKind<List, T> Empty<T>() => new List<T>(Enumerable.Empty<T>());

    public static TResult Fold<T, TResult>(IKind<List, T> foldable, TResult identity,
        Func<TResult, T, TResult> folder)
    {
        return foldable.To().SourceList.Aggregate(identity, folder);
    }

    public static IKind<TApplicative, IKind<List, V>> Traverse<T, V, TApplicative>(
        IKind<List, T> traversable,
        Func<T, IKind<TApplicative, V>> action) where TApplicative : IApplicative<TApplicative>
    {
        var list = Map(traversable.To(), action).To();
        return Fold(list,
            TApplicative.Pure(Empty<V>()),
            (acc, cur) =>
                IApplicative<TApplicative>.Lift2(Append,
                    acc, TApplicative.Map(cur, Pure)));
    }

    public static IKind<TApplicative, IKind<List, T>> Sequence<T, TApplicative>(
        IKind<List, IKind<TApplicative, T>> traversable)
        where TApplicative : IApplicative<TApplicative> =>
        ITraversable<List>.Sequence(traversable);

    public static IKind<List, T> Combine<T>(IKind<List, T> v1, IKind<List, T> v2) => Append(v1, v2);

    public static IKind<List, T> Identity<T>() => Empty<T>();
    
    public static IKind<List, T> Join<T>(IKind<List, IKind<List, T>> monad) => IMonad<List>.Join(monad);

    public static IKind<TMonadPlus, T> MSum<TMonadPlus, T>(IKind<List, IKind<TMonadPlus, T>> foldable)
        where TMonadPlus : IMonadPlus<TMonadPlus> =>
        IFoldable<List>.MSum(foldable);
    public static IKind<TAlternative, T> ASum<TAlternative, T>(IKind<List, IKind<TAlternative, T>> foldable)
        where TAlternative : IAlternative<TAlternative> =>
        IFoldable<List>.ASum(foldable);
}

public static class ListExt
{
    public static List<T> Prepend<T>(this IKind<List, T> list, T item) =>
        List.Prepend(item, list);
    public static List<T> To<T>(this IKind<List, T> list) => (List<T>)list;

    public static List<Z> SelectMany<T, V, Z>(this List<T> list, Func<T, List<V>> binder,
        Func<T, V, Z> projection)
    {
        return List.Bind(list, t => binder(t).Select(v => projection(t, v))).To();
    }

    public static List<V> Select<T, V>(this List<T> list, Func<T, V> mapper) =>
        List.Map(list, mapper).To();
}