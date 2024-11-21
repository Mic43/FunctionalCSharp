using FunctionalCSharp.New.Base;
using FunctionalCSharp.New.Monads;
using FunctionalCSharp.New.Monads.Free;
using FunctionalCSharp.SampleDSL.FirstLanguage;
using FunctionalCSharp.SampleDSL.NewLanguage;

namespace FunctionalCSharp.SampleDSL.CombinedLanguage;

internal static class CombinedInstructions
{
    internal static Free<Coproduct<Instruction<T>, NewInstruction>, TNext> ToCombined<T, TNext>(
        this Free<Instruction<T>, TNext> program) =>
        program.Hoist(new InstructionTransform<T>()).To();

    internal static Free<Coproduct<Instruction<T>, NewInstruction>, TNext> ToCombined<T, TNext>(
        this Free<NewInstruction, TNext> program) =>
        program.Hoist(new NewInstructionTransform<T>()).To();


    private class
        NewInstructionTransform<T> : INaturalTransformation<NewInstruction, Coproduct<Instruction<T>, NewInstruction>>
    {
        public IKind<Coproduct<Instruction<T>, NewInstruction>, TNext> Transform<TNext>(
            IKind<NewInstruction, TNext> functor) =>
            Coproduct<Instruction<T>, NewInstruction, TNext>.Right(functor);
    }

    private class
        InstructionTransform<T> : INaturalTransformation<Instruction<T>, Coproduct<Instruction<T>, NewInstruction>>
    {
        public IKind<Coproduct<Instruction<T>, NewInstruction>, TNext> Transform<TNext>(
            IKind<Instruction<T>, TNext> functor) =>
            Coproduct<Instruction<T>, NewInstruction, TNext>.Left(functor);
    }
}