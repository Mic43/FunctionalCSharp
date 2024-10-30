using System.Numerics;

namespace FunctionalCSharp.New;

public interface ISemigroup<TSemigroup> where TSemigroup : ISemigroup<TSemigroup>
{
    public static abstract IKind<TSemigroup, T> Combine<T>(IKind<TSemigroup, T> v1, IKind<TSemigroup, T> v2);
}

public interface IMonoid<TMonoid> : ISemigroup<TMonoid> where TMonoid : IMonoid<TMonoid>
{
    public static abstract IKind<TMonoid, T> Identity<T>();
}

public interface IFoldable<TFoldable> where TFoldable : IFoldable<TFoldable>
{
    // public static abstract IKind<TMonoid, TResult> FoldMap<TMonoid, T, TResult>(Func<T, IKind<TMonoid, TResult>> mapper,
    //     IKind<TFoldable, T> foldable);

    public static abstract TResult FoldBack<T, TResult>(IKind<TFoldable, T> foldable, TResult identity,
        Func<T, TResult, TResult> folder);
}