using System.Numerics;
using FunctionalCSharp.New.Monads;

namespace FunctionalCSharp.SampleDSL;

public interface IInterpreter
{
    public TOutput Interpret<T, TOutput>(Free<Instruction<T>, TOutput> program)
        where T : IMultiplyOperators<T, T, T>, IAdditionOperators<T, T, T>;
}