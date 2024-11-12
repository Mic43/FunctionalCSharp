using System.Numerics;
using FunctionalCSharp.New;
using FunctionalCSharp.New.Monads;

namespace FunctionalCSharp.SampleDSL;

abstract record CombinedLanguageInstruction<T, TNext> : IKind<CombinedLanguageInstruction<T>, TNext>;

record CInstruction<T, TNext>(Instruction<T, TNext> Instruction)
    : CombinedLanguageInstruction<T, TNext>;

record CNewInstruction<T, TNext>(NewInstruction<TNext> Instruction)
    : CombinedLanguageInstruction<T, TNext>;

abstract class CombinedLanguageInstruction<T> : IFunctor<CombinedLanguageInstruction<T>>
{
    public static IKind<CombinedLanguageInstruction<T>, VNext> Map<TNext, VNext>(
        IKind<CombinedLanguageInstruction<T>, TNext> instruction, Func<TNext, VNext> fun)

    {
        switch (instruction)
        {
            case CNewInstruction<T, TNext> cNewInstruction:
                return new CNewInstruction<T, VNext>(
                    (NewInstruction<VNext>)NewInstruction.Map(cNewInstruction.Instruction, fun));
            case CInstruction<T, TNext> cInstruction:
                return new CInstruction<T, VNext>(
                    (Instruction<T, VNext>)Instruction<T>.Map(cInstruction.Instruction, fun));
            default:
                throw new ArgumentOutOfRangeException(nameof(instruction));
        }
    }
}

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