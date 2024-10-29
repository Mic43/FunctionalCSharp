using System.Numerics;

namespace FunctionalCSharp.New;

public interface ISemigroup<TSemigroup> where TSemigroup : ISemigroup<TSemigroup>
{
    public static abstract TSemigroup Combine(TSemigroup v1, TSemigroup v2);
}

public interface IMonoid<TMonoid> : ISemigroup<TMonoid> where TMonoid : IMonoid<TMonoid>
{
    public static abstract TMonoid Identity();
}

public interface IFoldable<TFoldable> where TFoldable : IFoldable<TFoldable>
{
    public static abstract T Fold<T>(IKind<TFoldable, T> foldable) where T :
        IMonoid<T>;

    public static abstract T FoldAdd<T>(IKind<TFoldable, T> foldable)
        where T : IAdditionOperators<T, T, T>, IAdditiveIdentity<T, T>;

    public static abstract T FoldMul<T>(IKind<TFoldable, T> foldable)
        where T : IMultiplyOperators<T, T, T>, IMultiplicativeIdentity<T, T>;
}