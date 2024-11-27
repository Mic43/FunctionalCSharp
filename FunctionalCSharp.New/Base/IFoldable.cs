namespace FunctionalCSharp.New.Base;

public interface IFoldable<TFoldable> where TFoldable : IFoldable<TFoldable>
{
    public static abstract TResult Fold<T, TResult>(IKind<TFoldable, T> foldable, TResult identity,
        Func<TResult, T, TResult> folder);

    public static IKind<TMonadPlus, T> MSum<TMonadPlus, T>(IKind<TFoldable, IKind<TMonadPlus, T>> foldable)
        where TMonadPlus : IMonadPlus<TMonadPlus> =>
        TFoldable.Fold(foldable, TMonadPlus.Empty<T>(),
            TMonadPlus.Append);

    public static IKind<TAlternative, T> ASum<TAlternative, T>(IKind<TFoldable, IKind<TAlternative, T>> foldable)
        where TAlternative : IAlternative<TAlternative> =>
        TFoldable.Fold(foldable, TAlternative.Empty<T>(),
            TAlternative.Append);
}