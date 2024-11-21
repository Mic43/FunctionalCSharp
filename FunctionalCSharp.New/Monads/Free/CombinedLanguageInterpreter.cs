using FunctionalCSharp.New.Base;

namespace FunctionalCSharp.New.Monads.Free;

public class CombinedLanguageInterpreter<TOutput, TLanguageA, TLanguageB>(
    FreeInterpreterBase<TOutput, TLanguageA> languageAInterpreter,
    FreeInterpreterBase<TOutput, TLanguageB> languageBInterpreter
) :FreeInterpreterBase<TOutput,Coproduct<TLanguageA,TLanguageB>>
    where TLanguageA : IFunctor<TLanguageA> where TLanguageB : IFunctor<TLanguageB>
{
    public override TNext InterpretSingle<TNext>(IKind<Coproduct<TLanguageA, TLanguageB>, TNext> instruction)
    {
        return instruction.To().Match(
            languageAInterpreter.InterpretSingle,
            languageBInterpreter.InterpretSingle);
    }
}