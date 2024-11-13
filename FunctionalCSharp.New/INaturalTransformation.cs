using FunctionalCSharp.New.Monads;

namespace FunctionalCSharp.New;

public interface INaturalTransformation<TFunctorA,TFunctorB>
    where TFunctorA:IFunctor<TFunctorA>
    where TFunctorB:IFunctor<TFunctorB>
{
    public IKind<TFunctorB, T> Transform<T>(IKind<TFunctorA, T> functor);
}
