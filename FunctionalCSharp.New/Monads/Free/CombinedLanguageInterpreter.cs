using FunctionalCSharp.New.Base;

namespace FunctionalCSharp.New.Monads.Free;

public class CombinedLanguageInterpreter<TOutput, TLanguageA, TLanguageB>(
    FreeInterpreterBase<TOutput, TLanguageA> languageAInterpreter,
    FreeInterpreterBase<TOutput, TLanguageB> languageBInterpreter
) where TLanguageA : IFunctor<TLanguageA> where TLanguageB : IFunctor<TLanguageB>
{
    private TNext InterpretSingle<TNext>(IKind<Coproduct<TLanguageA, TLanguageB>, TNext> instruction)
    {
        return instruction.To().Match(
            languageAInterpreter.InterpretSingle,
            languageBInterpreter.InterpretSingle);
    }

    public TOutput Interpret(Free<Coproduct<TLanguageA, TLanguageB>, TOutput> program)
    {
        return Free<Coproduct<TLanguageA, TLanguageB>>.Iter(InterpretSingle, program);
    }
}