using System.Numerics;

namespace FunctionalCSharp.New.Monads;

public record List<T>(IEnumerable<T> SourceList) : IKind<List, T>
{
    public List() : this(Enumerable.Empty<T>())
    {
    }

    public IEnumerable<T> SourceList { get; } = SourceList;
}

public class List : IMonadPlus<List>, IFoldable<List>
{
    public static IKind<List, V> Map<T, V>(IKind<List, T> f, Func<T, V> fun)
    {
        return IMonad<List>.Map(f, fun);
    }

    public static IKind<List, V> Apply<T, V>(IKind<List, T> applicative, IKind<List, Func<T, V>> fun)
    {
        return IMonad<List>.Apply(applicative, fun);
    }

    public static IKind<List, V> Bind<T, V>(IKind<List, T> monad, Func<T, IKind<List, V>> fun)
    {
        return new List<V>(monad.To().SourceList.SelectMany(t => fun(t).To().SourceList));
    }

    public static IKind<List, T> Pure<T>(T value)
    {
        return new List<T>(Enumerable.Repeat(value, 1));
    }

    public static IKind<List, T> Append<T, V>(IKind<List, T> a, IKind<List, T> b)
    {
        return new List<T>(a.To().SourceList.Append<>(b.To().SourceList));
    }

    public static IKind<List, T> Empty<T>()
    {
        return new List<T>(Enumerable.Empty<T>());
    }

    public static T Fold<T>(IKind<List, T> foldable) where T : IMonoid<T>
    {
        return foldable.To().SourceList.Aggregate(T.Identity(), T.Combine);
    }

    public static T FoldAdd<T>(IKind<List, T> foldable) where T : IAdditionOperators<T, T, T>, IAdditiveIdentity<T, T>
    {
        return foldable.To().SourceList.Aggregate(T.AdditiveIdentity, (acc, v) => acc + v);
    }

    public static T FoldMul<T>(IKind<List, T> foldable)
        where T : IMultiplyOperators<T, T, T>, IMultiplicativeIdentity<T, T>
    {
        return foldable.To().SourceList.Aggregate(T.MultiplicativeIdentity, (acc, v) => acc * v);
    }
}

public static class ListExt
{
    public static List<T> To<T>(this IKind<List, T> list)
    {
        return (List<T>)list;
    }

    public static List<Z> SelectMany<T, V, Z>(this List<T> list, Func<T, List<V>> binder,
        Func<T, V, Z> projection)
    {
        return List.Bind(list, t => List.Bind(binder(t), v => List.Pure(projection(t, v)))).To();
    }

    public static List<V> Select<T, V, TEnv>(this List<T> list, Func<T, V> mapper)
    {
        return List.Map(list, mapper).To();
    }
}