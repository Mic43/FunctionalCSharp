using System.Numerics;
using FunctionalCSharp.New;
using FunctionalCSharp.New.Monads;

namespace FunctionalCSharp.SampleDSL;

public interface IInstructionsInterpreter
{
    public TOutput Interpret<T, TOutput>(Free<Instruction<T>, TOutput> program)
        where T : IMultiplyOperators<T, T, T>, IAdditionOperators<T, T, T>;

    public TNext InterpretSingle<T, TNext>(IKind<Instruction<T>, TNext> instruction)
        where T : IMultiplyOperators<T, T, T>, IAdditionOperators<T, T, T>;
}