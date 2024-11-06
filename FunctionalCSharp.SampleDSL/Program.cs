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
            from _ in Instructions.WriteLine<int>("Podaj liczbe")
            from input in Instructions.ReadLine<int>()
            from z in Instructions.If(() => r2 > input,
                Instructions.Const(a),
                Instructions.Const(b))
            select z;

        IInterpreter interpreter = new BasicInterpreter();
        var result = interpreter.Interpret(program);

        Console.WriteLine(result);
    }
}