namespace FunctionalCSharp.New.Base;

public interface IMonoid<TMonoid> : ISemigroup<TMonoid> where TMonoid : IMonoid<TMonoid>
{
    public static abstract IKind<TMonoid, T> Identity<T>();
}