using System;
using System.Numerics;
using FunctionalCSharp.New;
using FunctionalCSharp.New.Monads;
using FunctionalCSharp.SampleDSL;
using Xunit;
using Xunit.Abstractions;

namespace FunctionalCSharp.Tests;

public class FreeDslTest
{
    private readonly ITestOutputHelper _testOutputHelper;

    public FreeDslTest(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    [Fact]
    void Tst()
    {
        int a = 1;
        int b = 2;
        int c = 3;
        
        var program =
            from r1 in Instructions.Add(a, b)
            from r2 in Instructions.Mul(r1, c)
            from z in Instructions.If(() => r1 > r2,
                Instructions.Const(a),
                Instructions.Const(b))
            select z;

        IInstructionsInterpreter interpreter = new InstructionsInterpreter();
        // IInterpreter interpreter = new LoggingInterpreter(new BasicInterpreter(), _testOutputHelper);
        var result = interpreter.Interpret(program);

        Assert.Equal((a + b) > (a + b) * c ? a : b, result);
    }
}