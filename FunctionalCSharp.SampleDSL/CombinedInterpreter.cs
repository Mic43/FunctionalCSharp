using FunctionalCSharp.New;

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