namespace FunctionalCSharp.New.Base;

/// <summary>
/// Natural transformation between two functors
/// </summary>
/// <typeparam name="TFunctorA"></typeparam>
/// <typeparam name="TFunctorB"></typeparam>
public interface INaturalTransformation<in TFunctorA,out TFunctorB>
    where TFunctorA:IFunctor<TFunctorA>
    where TFunctorB:IFunctor<TFunctorB>
{
    public IKind<TFunctorB, T> Transform<T>(IKind<TFunctorA, T> functor);
}
    