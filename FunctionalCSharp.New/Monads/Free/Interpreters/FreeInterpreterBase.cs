using FunctionalCSharp.New.Base;

namespace FunctionalCSharp.New.Monads.Free.Interpreters;

public abstract class FreeInterpreterBase<TOutput, Functor> : IFreeInterpreter<TOutput, Functor>
    where Functor : IFunctor<Functor>
{
    public virtual TOutput Interpret(Free<Functor, TOutput> program)
    {
        return Free<Functor>.Iter(InterpretSingle, program);
    }

    public abstract TNext InterpretSingle<TNext>(IKind<Functor, TNext> command);
}