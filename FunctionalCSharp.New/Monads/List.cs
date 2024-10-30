using System.Numerics;
using System.Runtime.CompilerServices;

namespace FunctionalCSharp.New.Monads;

public record List<T>(IEnumerable<T> SourceList) : IKind<List, T>
{
    public List() : this(Enumerable.Empty<T>())
    {
    }

    public IEnumerable<T> SourceList { get; } = SourceList;
}

public class List : IMonadPlus<List>, ITraversable<List>
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
        return new List<T>(a.To().SourceList.Union(b.To().SourceList));
    }

    public static IKind<List, T> Empty<T>()
    {
        return new List<T>(Enumerable.Empty<T>());
    }

    public static TResult FoldBack<T, TResult>(IKind<List, T> foldable, TResult identity,
        Func<T, TResult, TResult> folder)
    {
        //TODO: not efficient
        return foldable.To().SourceList.Reverse().Aggregate(identity, (result, value) => folder(value, result));
    }

    public static IKind<TApplicative, IKind<List, V>> Traverse<T, V, TApplicative>(
        IKind<List, T> traversable,
        Func<T, IKind<TApplicative, V>> action) where TApplicative : IApplicative<TApplicative>
    {
        var list = Map(traversable.To(), action).To();
        return FoldBack(list,
            TApplicative.Pure(Empty<V>()),
            (cur, acc) =>
                IApplicative<TApplicative>.Lift2(Append<V, V>
                    , TApplicative.Map(cur, Pure), acc));
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