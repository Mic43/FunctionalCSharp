namespace FunctionalCSharp.New;

public interface ITraversable<TTraversable> : IFoldable<TTraversable>, IFunctor<TTraversable>
    where TTraversable : ITraversable<TTraversable>
{
    public static abstract IKind<TApplicative, IKind<TTraversable, V>> Traverse<T, V, TApplicative>(
        IKind<TTraversable, T> traversable, Func<T, IKind<TApplicative, V>> action)
        where TApplicative : IApplicative<TApplicative>;

    public static IKind<TApplicative, IKind<TTraversable, T>> Sequence<T, TApplicative>(
        IKind<TTraversable, IKind<TApplicative, T>> traversable)
        where TApplicative : IApplicative<TApplicative> =>
        TTraversable.Traverse(traversable, v => v);
}