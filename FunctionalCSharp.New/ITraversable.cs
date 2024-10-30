namespace FunctionalCSharp.New;

public interface ITraversable<TTraversable> : IFoldable<TTraversable>, IFunctor<TTraversable>
    where TTraversable : ITraversable<TTraversable>
{
    public static abstract IKind<TApplicative, IKind<TTraversable, V>> Traverse<T, V, TApplicative>(
        IKind<TTraversable, T> traversable, Func<T, IKind<TApplicative, V>> action)
        where TApplicative : IApplicative<TApplicative>;
}