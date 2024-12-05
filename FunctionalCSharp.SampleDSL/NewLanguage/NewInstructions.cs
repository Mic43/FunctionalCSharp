using FunctionalCSharp.New;
using FunctionalCSharp.New.Base;
using FunctionalCSharp.New.Monads;
using FunctionalCSharp.New.Monads.Free;

namespace FunctionalCSharp.SampleDSL.NewLanguage;

abstract record NewInstruction<TNext> : IKind<NewInstruction, TNext>;

record WriteLine<TNext>(string Line, Func<TNext> Next) : NewInstruction<TNext>;

record ReadLine<TNext>(Func<string, TNext> Next) : NewInstruction<TNext>;

static class NewInstructions
{
    public static Free<NewInstruction, Unit> WriteLine(string line) =>
        Free<NewInstruction>.LiftF(new WriteLine<Unit>(line, Unit.Instance));

    public static Free<NewInstruction, string> ReadLine() =>
        Free<NewInstruction>.LiftF(new ReadLine<string>(s => s));
}

abstract class NewInstruction : IFunctor<NewInstruction>
{
    public static IKind<NewInstruction, VNext> Map<TNext, VNext>(IKind<NewInstruction, TNext> f, Func<TNext, VNext> fun)
    {
        return (NewInstruction<TNext>)f switch
        {
            ReadLine<TNext> readLine => new ReadLine<VNext>(readLine.Next.Compose(fun)),
            WriteLine<TNext> writeLine => new WriteLine<VNext>(writeLine.Line, () => fun(writeLine.Next())),
            _ => throw new ArgumentOutOfRangeException(nameof(f))
        };
    }
}