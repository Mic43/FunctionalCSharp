using System.Numerics;

namespace FunctionalCSharp.New
{
    public interface IFoldable<TFoldable> where TFoldable : IFoldable<TFoldable>
    {
        public static abstract T Fold<T>(T m, IKind<TFoldable, T> foldable) where T :
            IAdditionOperators<T, T, T>, IAdditiveIdentity<T, T>;
    }
}
