using System.Security.Cryptography;
using FunctionalCSharp.New;
using FunctionalCSharp.New.Monads;
using FunctionalCSharp.SampleDSL.FirstLanguage;
using FunctionalCSharp.SampleDSL.NewLanguage;

namespace FunctionalCSharp.SampleDSL.CombinedLanguage;

abstract record CombinedLanguageInstruction<T, TNext> : IKind<CombinedLanguageInstruction<T>, TNext>;

record CInstruction<T, TNext>(Instruction<T, TNext> Instruction)
    : CombinedLanguageInstruction<T, TNext>;

record CNewInstruction<T, TNext>(NewInstruction<TNext> Instruction)
    : CombinedLanguageInstruction<T, TNext>;

/// <summary>
/// Utility functions 
/// </summary>
internal static class CombinedInstructions
{
    private class InstructionTransform<T> : INaturalTransformation<Instruction<T>, CombinedLanguageInstruction<T>>
    {
        public IKind<CombinedLanguageInstruction<T>, TNext> Transform<TNext>(IKind<Instruction<T>, TNext> functor) =>
            new CInstruction<T, TNext>((Instruction<T, TNext>)functor);
    }

    private class NewInstructionTransform<T> : INaturalTransformation<NewInstruction, CombinedLanguageInstruction<T>>
    {
        public IKind<CombinedLanguageInstruction<T>, TNext> Transform<TNext>(IKind<NewInstruction, TNext> functor)
        {
            return new CNewInstruction<T, TNext>((NewInstruction<TNext>)functor);
        }
    }

    internal static Free<CombinedLanguageInstruction<T>, TNext> ToCombined<T, TNext>(
        this Free<Instruction<T>, TNext> program) =>
        program.Hoist(new InstructionTransform<T>()).To();

    internal static Free<CombinedLanguageInstruction<T>, TNext> ToCombined<T, TNext>(
        this Free<NewInstruction, TNext> program) =>
        program.Hoist(new NewInstructionTransform<T>()).To();
}

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