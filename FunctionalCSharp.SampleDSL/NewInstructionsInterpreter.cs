using System.Numerics;
using FunctionalCSharp.New;
using FunctionalCSharp.New.Monads;

namespace FunctionalCSharp.SampleDSL;

interface INewInstructionsInterpreter
{
    public TOutput Interpret<TOutput>(Free<NewInstruction, TOutput> program)
        => Free<NewInstruction>.Iter(InterpretSingle, program);
    public TNext InterpretSingle<TNext>(IKind<NewInstruction, TNext> instruction);
}

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