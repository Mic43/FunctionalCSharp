using System;
using System.Numerics;
using FunctionalCSharp.New;
using FunctionalCSharp.New.Monads;
using Xunit;

namespace FunctionalCSharp.Tests;

abstract record Instruction<T, TNext> : IKind<Instruction<T>, TNext>
    where T : IAdditionOperators<T, T, T>, IMultiplyOperators<T, T, T>;

sealed record Add<T, TNext>(T Arg1, T Arg2, Func<T, TNext> Next)
    : Instruction<T, TNext> where T : IAdditionOperators<T, T, T>, IMultiplyOperators<T, T, T>;

sealed record Mul<T, TNext>(T Arg1, T Arg2, Func<T, TNext> Next)
    : Instruction<T, TNext> where T : IMultiplyOperators<T, T, T>, IAdditionOperators<T, T, T>;

/// <summary>
/// Functor implementation
/// </summary>
/// <typeparam name="T"></typeparam>
class Instruction<T> : FunctionalCSharp.New.IFunctor<Instruction<T>>
    where T : IAdditionOperators<T, T, T>, IMultiplyOperators<T, T, T>
{
    public static IKind<Instruction<T>, VNext> Map<TNext, VNext>(IKind<Instruction<T>, TNext> f, Func<TNext, VNext> fun)
    {
        var ff = (Instruction<T, TNext>)f;
        return ff switch
        {
            Add<T, TNext> add => new Add<T, VNext>(add.Arg1, add.Arg2, t => fun(add.Next(t))),
            Mul<T, TNext> mul => new Mul<T, VNext>(mul.Arg1, mul.Arg2, t => fun(mul.Next(t))),
            _ => throw new ArgumentOutOfRangeException(nameof(f))
        };
    }
}

static class Instructions
{
    public static Free<Instruction<T>, T> Add<T>(T arg1, T arg2)
        where T : IAdditionOperators<T, T, T>, IMultiplyOperators<T, T, T> =>
        Free<Instruction<T>>.LiftF(new Add<T, T>(arg1, arg2, z => z)).To();
    
    public static Free<Instruction<T>, T> Mul<T>(T arg1, T arg2)
        where T : IAdditionOperators<T, T, T>, IMultiplyOperators<T, T, T> =>
        Free<Instruction<T>>.LiftF(new Mul<T, T>(arg1, arg2, z => z)).To();
}


internal interface IInterpreter
{
    public TOutput Interpret<T, TOutput>(Free<Instruction<T>, TOutput> program)
        where T : IMultiplyOperators<T, T, T>, IAdditionOperators<T, T, T>;
    
}

class BasicInterpreter : IInterpreter
{
    public TOutput Interpret<T, TOutput>(Free<Instruction<T>, TOutput> program)
        where T : IMultiplyOperators<T, T, T>, IAdditionOperators<T, T, T>
    {
        return program switch
        {
            Pure<Instruction<T>, TOutput> pure => pure.Value,
            Roll<Instruction<T>, TOutput> roll => (Instruction<T, Free<Instruction<T>, TOutput>>)roll.Free switch
            {
                Add<T, Free<Instruction<T>, TOutput>> add => Interpret(add.Next(add.Arg1 + add.Arg2)),
                Mul<T, Free<Instruction<T>, TOutput>> mul => Interpret(mul.Next(mul.Arg1 * mul.Arg2)),
                _ => throw new ArgumentOutOfRangeException(nameof(program))
            },
            _ => throw new ArgumentOutOfRangeException(nameof(program))
        };
    }
}

public class FreeDslTest
{
    [Fact]
    void Tst()
    {
        int a = 1;
        int b = 2;
        int c = 3;
        var program =
            from r1 in Instructions.Add(a, b)
            from r2 in Instructions.Mul(r1, c)
            select r2;

        IInterpreter interpreter = new BasicInterpreter();
        var result = interpreter.Interpret(program);

        Assert.Equal((a + b) * c, result);
    }
}