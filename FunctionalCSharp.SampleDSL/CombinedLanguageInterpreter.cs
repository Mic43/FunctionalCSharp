using System.Numerics;
using FunctionalCSharp.New;
using FunctionalCSharp.New.Monads;

namespace FunctionalCSharp.SampleDSL;

class CombinedLanguageInterpreter(
    IInstructionsInterpreter instructionsInterpreter,
    INewInstructionsInterpreter newInstructionsInterpreter)
{
    private TNext InterpretSingle<T, TNext>(IKind<CombinedLanguageInstruction<T>, TNext> instruction)
        where T : IMultiplyOperators<T, T, T>, IAdditionOperators<T, T, T>
    {
        return (CombinedLanguageInstruction<T, TNext>)instruction switch
        {
            CNewInstruction<T, TNext> cNewInstruction => newInstructionsInterpreter.InterpretSingle(cNewInstruction
                .Instruction),
            CInstruction<T, TNext> cInstruction => instructionsInterpreter.InterpretSingle(cInstruction.Instruction),
            _ => throw new ArgumentOutOfRangeException(nameof(instruction))
        };
    }

    public TOutput Interpret<T, TOutput>(Free<CombinedLanguageInstruction<T>, TOutput> program)
        where T : IMultiplyOperators<T, T, T>, IAdditionOperators<T, T, T>
    {
        return Free<CombinedLanguageInstruction<T>>.Iter(InterpretSingle, program);
    }
}