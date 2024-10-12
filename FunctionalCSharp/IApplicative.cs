using System;

namespace FunctionalCSharp
{
    public interface IApplicative<T> : IFunctor<T>
    {   
        IApplicative<V> Apply<V>(IApplicative<Func<T, V>> fun);
    }
}