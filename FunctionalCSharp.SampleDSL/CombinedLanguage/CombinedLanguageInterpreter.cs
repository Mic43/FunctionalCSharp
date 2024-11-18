using System.Numerics;
using FunctionalCSharp.New;
using FunctionalCSharp.New.Base;
using FunctionalCSharp.New.Monads;
using FunctionalCSharp.SampleDSL.FirstLanguage;
using FunctionalCSharp.SampleDSL.NewLanguage;

namespace FunctionalCSharp.SampleDSL.CombinedLanguage;


class CombinedLanguageInterpreter(
    IInstructionsInterpreter instructionsInterpreter,
    INewInstructionsInterpreter newInstructionsInterpreter)
{
    private TNext InterpretSingle<T, TNext>(IKind<Coproduct<Instruction<T>, NewInstruction>, TNext> instruction)
        where T : IMultiplyOperators<T, T, T>, IAdditionOperators<T, T, T>
    {
        return instruction.To().Match(
            instruction => instructionsInterpreter.InterpretSingle((Instruction<T, TNext>)instruction),
            instruction => newInstructionsInterpreter.InterpretSingle((NewInstruction<TNext>)instruction));
        
    }

    public TOutput Interpret<T, TOutput>(Free<Coproduct<Instruction<T>, NewInstruction>, TOutput> program)
        where T : IMultiplyOperators<T, T, T>, IAdditionOperators<T, T, T>
    {
        return Free<Coproduct<Instruction<T>, NewInstruction>>.Iter(InterpretSingle, program);
    }
}