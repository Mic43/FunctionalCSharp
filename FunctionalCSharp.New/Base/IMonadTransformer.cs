namespace FunctionalCSharp.New.Base;
/// <summary>
/// Monad transformer base interface
/// </summary>
/// <typeparam name="TMonadTransformer">Concrete transfomer class</typeparam>
/// <typeparam name="TMonad">base monad used by this transfomer </typeparam>
public interface IMonadTransformer<out TMonadTransformer, in TMonad>
{
    public static abstract IKind<TMonadTransformer, T> Lift<T>(IKind<TMonad, T> monad);
}