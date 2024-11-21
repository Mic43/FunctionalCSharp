using FunctionalCSharp.New;
using FunctionalCSharp.New.Base;
using FunctionalCSharp.New.Monads;
using FunctionalCSharp.New.Monads.Free;

namespace FunctionalCSharp.SampleDSL.NewLanguage;

interface INewInstructionsInterpreter
{
    public TOutput Interpret<TOutput>(Free<NewInstruction, TOutput> program)
        => Free<NewInstruction>.Iter(InterpretSingle, program);
    public TNext InterpretSingle<TNext>(IKind<NewInstruction, TNext> instruction);
}