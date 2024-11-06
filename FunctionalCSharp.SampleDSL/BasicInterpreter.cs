using System.Numerics;
using FunctionalCSharp.New.Monads;

namespace FunctionalCSharp.SampleDSL;

public class BasicInterpreter : IInterpreter
{
    public TOutput Interpret<T, TOutput>(Free<Instruction<T>, TOutput> program)
        where T : IMultiplyOperators<T, T, T>, IAdditionOperators<T, T, T>
    {
        switch (program)
        {
            case Pure<Instruction<T>, TOutput> pure:
                return pure.Value;
            case Roll<Instruction<T>, TOutput> roll:
                switch ((Instruction<T, Free<Instruction<T>, TOutput>>)roll.Free)
                {
                    case Add<T, Free<Instruction<T>, TOutput>> add:
                        return Interpret(add.Next(add.Arg1 + add.Arg2));
                    case If<T, Free<Instruction<T>, TOutput>> @if:
                        return Interpret(@if.Condition() ? Interpret(@if.OnTrue) : Interpret(@if.OnFalse));
                    case Mul<T, Free<Instruction<T>, TOutput>> mul:
                        return Interpret(mul.Next(mul.Arg1 * mul.Arg2));
                    case ReadLine<T, Free<Instruction<T>, TOutput>> readLine:
                        return Interpret(readLine.Next((T)Convert.ChangeType(Console.ReadLine(), typeof(T))! ??
                                                       throw new InvalidOperationException()));
                    case WriteLine<T, Free<Instruction<T>, TOutput>> writeLine:
                        Console.WriteLine(writeLine.Line);
                        return Interpret(writeLine.Next());
                    default:
                        throw new ArgumentOutOfRangeException(nameof(program));
                }
            default:
                throw new ArgumentOutOfRangeException(nameof(program));
        }
    }
}

// class LoggingInterpreter(IInterpreter innerInterpret, ITestOutputHelper testOutputHelper) : IInterpreter
// {
//     public TOutput Interpret<T, TOutput>(Free<Instruction<T>, TOutput> program)
//         where T : IMultiplyOperators<T, T, T>, IAdditionOperators<T, T, T>
//     {
//         switch (program)
//         {
//             case Pure<Instruction<T>, TOutput> pure:
//                 testOutputHelper.WriteLine($" {pure.Value.ToString()} ");
//                 return innerInterpret.Interpret(pure);
//             case Roll<Instruction<T>, TOutput> roll:
//                 switch ((Instruction<T, Free<Instruction<T>, TOutput>>)roll.Free)
//                 {
//                     case Add<T, Free<Instruction<T>, TOutput>> add:
//                         testOutputHelper.WriteLine("Add: " + add.Arg1 + " " + add.Arg2);
//                         return innerInterpret.Interpret(roll);
//                     case Mul<T, Free<Instruction<T>, TOutput>> mul:
//                         testOutputHelper.WriteLine("Mul: " + mul.Arg1 + " " + mul.Arg2);
//                         return innerInterpret.Interpret(roll);
//                     default:
//                         throw new ArgumentOutOfRangeException(nameof(program));
//                 }
//             default:
//                 throw new ArgumentOutOfRangeException(nameof(program));
//         }
//     }
//}