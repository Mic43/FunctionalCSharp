namespace FunctionalCSharp.New;

public interface IMonoid<TMonoid> : ISemigroup<TMonoid> where TMonoid : IMonoid<TMonoid>
{
    public static abstract IKind<TMonoid, T> Identity<T>();
}