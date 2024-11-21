using System.Numerics;
using FunctionalCSharp.New;
using FunctionalCSharp.New.Base;
using FunctionalCSharp.New.Monads;
using FunctionalCSharp.New.Monads.Free;

namespace FunctionalCSharp.SampleDSL.FirstLanguage;

public class InstructionsInterpreter : IInstructionsInterpreter
{
    public TOutput Interpret<T, TOutput>(Free<Instruction<T>, TOutput> program)
        where T : IMultiplyOperators<T, T, T>, IAdditionOperators<T, T, T>
        => Free<Instruction<T>>.Iter(InterpretSingle, program);

    public TNext InterpretSingle<T, TNext>(IKind<Instruction<T>, TNext> instruction)
        where T : IMultiplyOperators<T, T, T>, IAdditionOperators<T, T, T>
    {
        switch (instruction)
        {
            case Dynamic<T, TNext> dynamic:
                return Interpret(dynamic.Program.Invoke());
            case Add<T, TNext> add:
                return add.Next(add.Arg1 + add.Arg2);
            case If<T, TNext> @if:
                return @if.Condition() ? Interpret(@if.OnTrue) : Interpret(@if.OnFalse);
            case Mul<T, TNext> mul:
                return mul.Next(mul.Arg1 * mul.Arg2);
            case Read<T, TNext> read:
                return read.Next((T)Convert.ChangeType(Console.ReadLine(), typeof(T))! ??
                                 throw new InvalidOperationException());
            case Try<T, TNext> @try:
                try
                {
                    return Interpret(@try.Guarded);
                }
                catch (Exception _)
                {
                    return Interpret(@try.OnCatch);
                }
            case Write<T, TNext> write:
                Console.Write(write.Value);
                return write.Next();
            default:
                throw new ArgumentOutOfRangeException(nameof(instruction));
        }
    }
}