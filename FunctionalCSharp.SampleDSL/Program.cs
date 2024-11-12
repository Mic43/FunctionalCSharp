using System.Numerics;
using System.Security.AccessControl;
using FunctionalCSharp.New;
using FunctionalCSharp.New.Monads;

namespace FunctionalCSharp.SampleDSL;

public class Tst
{
    static void Main()
    {
        int a = 1;
        int b = 2;
        int c = 3;

        var program =
            from r1 in Instructions.Add(a, b)
            from r2 in Instructions.Mul(r1, c)
            from _ in Instructions.Write(r1)
            from input in Instructions.Read<int>()
            from z in Instructions.If(() => r2 > input,
                 Instructions.Const(a),
                 Instructions.Const(b))
           
            select z;
        
        IInstructionsInterpreter interpreter = new InstructionsInterpreter();
        var result = interpreter.Interpret(program);
        //
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