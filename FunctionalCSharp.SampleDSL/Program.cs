using System.Numerics;
using System.Security.AccessControl;
using FunctionalCSharp.New;
using FunctionalCSharp.New.Monads;
using FunctionalCSharp.SampleDSL.CombinedLanguage;
using FunctionalCSharp.SampleDSL.FirstLanguage;
using FunctionalCSharp.SampleDSL.NewLanguage;

namespace FunctionalCSharp.SampleDSL;

public class Tst
{
    static void Main()
    {
        int a = 1;
        int b = 2;
        int c = 3;

        Free<Instruction<int>, int> Ask()
        {
            return from number in Instructions.Try(
                    Instructions.Read<int>(),
                    Instructions.Dynamic(Ask))
                select number;
        }

        var program =
            from r1 in Instructions.Add(a, b)
            from r2 in Instructions.Mul(r1, c)
            from _ in Instructions.Write(r1)
            from input in Ask()
            from z in Instructions.If(() => r2 > input,
                Instructions.Const(a),
                Instructions.Const(b))
            select z;

        IInstructionsInterpreter interpreter = new InstructionsInterpreter();
        var result = interpreter.Interpret(program);
        //
        Console.WriteLine(result);

        var program2 =
            from z in Instructions.Add(a, b).ToCombined()
            from vv in Instructions.Add(z, b).ToCombined()
            from _ in NewInstructions.WriteLine("aaaa" + vv).ToCombined<int, Unit>()
            select z;

        CombinedLanguageInterpreter i = new CombinedLanguageInterpreter(interpreter, new NewInstructionsInterpreter());
        result = i.Interpret(program2);
        Console.WriteLine(result);

        // var program2 = 
        //     from r1 in Instructions.Add(a, b)
        //     from r2 in Instructions.Mul(r1, c)
        //     select r2;
        //
        // var interpret = new PrettyPrintInterpreter().Interpret(program2);
        // Console.WriteLine(interpret);
    }
}