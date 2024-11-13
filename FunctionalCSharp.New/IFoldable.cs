using System.Numerics;

namespace FunctionalCSharp.New;

public interface IFoldable<TFoldable> where TFoldable : IFoldable<TFoldable>
{
    // public static abstract IKind<TMonoid, TResult> FoldMap<TMonoid, T, TResult>(Func<T, IKind<TMonoid, TResult>> mapper,
    //     IKind<TFoldable, T> foldable);

    public static abstract TResult Fold<T, TResult>(IKind<TFoldable, T> foldable, TResult identity,
        Func<TResult,T, TResult> folder);
}