using FunctionalCSharp.New.Monads;

namespace FunctionalCSharp.New;

/// <summary>
/// Natural transformation between two functors
/// </summary>
/// <typeparam name="TFunctorA"></typeparam>
/// <typeparam name="TFunctorB"></typeparam>
public interface INaturalTransformation<TFunctorA,TFunctorB>
    where TFunctorA:IFunctor<TFunctorA>
    where TFunctorB:IFunctor<TFunctorB>
{
    public IKind<TFunctorB, T> Transform<T>(IKind<TFunctorA, T> functor);
}
