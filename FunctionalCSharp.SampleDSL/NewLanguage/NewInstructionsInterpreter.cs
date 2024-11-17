using FunctionalCSharp.New;
using FunctionalCSharp.New.Base;

namespace FunctionalCSharp.SampleDSL.NewLanguage;

class NewInstructionsInterpreter : INewInstructionsInterpreter
{
    public TNext InterpretSingle<TNext>(IKind<NewInstruction, TNext> instruction)
    {
        switch (instruction)
        {
            case ReadLine<TNext> readLine:
                return readLine.Next(Console.ReadLine());
            case WriteLine<TNext> writeLine:
                Console.WriteLine(writeLine.Line);
                return writeLine.Next();
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}