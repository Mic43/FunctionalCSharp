using FunctionalCSharp.New;
using FunctionalCSharp.New.Monads;

namespace FunctionalCSharp.SampleDSL.NewLanguage;

interface INewInstructionsInterpreter
{
    public TOutput Interpret<TOutput>(Free<NewInstruction, TOutput> program)
        => Free<NewInstruction>.Iter(InterpretSingle, program);
    public TNext InterpretSingle<TNext>(IKind<NewInstruction, TNext> instruction);
}