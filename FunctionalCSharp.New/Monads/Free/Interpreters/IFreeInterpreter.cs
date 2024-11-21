using FunctionalCSharp.New.Base;

namespace FunctionalCSharp.New.Monads.Free.Interpreters;

interface IFreeInterpreter<TOutput, Functor> where Functor : IFunctor<Functor>
{
    public TOutput Interpret(Free<Functor, TOutput> program);
}