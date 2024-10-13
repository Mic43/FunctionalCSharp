using System;

namespace FunctionalCSharp
{
    public interface IFunctor<out T>
    {
       // First problem:
       // As fact, c# type system cannot bound returned functors to the same implementation as instance type       
       IFunctor<V> Map<V>(Func<T, V> mapper);     
    }
}