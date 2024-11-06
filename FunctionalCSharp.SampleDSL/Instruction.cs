using System.Numerics;
using FunctionalCSharp.New;
using FunctionalCSharp.New.Monads;

namespace FunctionalCSharp.SampleDSL;

// Sample DSL instructions
abstract record Instruction<T, TNext> : IKind<Instruction<T>, TNext>;

sealed record Add<T, TNext>(T Arg1, T Arg2, Func<T, TNext> Next)
    : Instruction<T, TNext>;

sealed record Mul<T, TNext>(T Arg1, T Arg2, Func<T, TNext> Next)
    : Instruction<T, TNext>;

sealed record ReadLine<T, TNext>(Func<T, TNext> Next)
    : Instruction<T, TNext>;

sealed record WriteLine<T, TNext>(string Line, Func<TNext> Next)
    : Instruction<T, TNext>;

sealed record If<T, TNext>(
    Func<bool> Condition,
    Free<Instruction<T>, TNext> OnTrue,
    Free<Instruction<T>, TNext> OnFalse,
    Func<T, TNext> Next)
    : Instruction<T, TNext>;

/// <summary>
/// Helper class for creating DSL instructions
/// </summary>
public static class Instructions
{
    public static Free<Instruction<T>, T> Add<T>(T arg1, T arg2)
        => Free<Instruction<T>>.LiftF(new Add<T, T>(arg1, arg2, z => z)).To();

    public static Free<Instruction<T>, T> Mul<T>(T arg1, T arg2) =>
        Free<Instruction<T>>.LiftF(new Mul<T, T>(arg1, arg2, z => z)).To();

    public static Free<Instruction<T>, T> If<T>(Func<bool> condition,
        Free<Instruction<T>, T> onTrue,
        Free<Instruction<T>, T> onFalse) =>
        Free<Instruction<T>>.LiftF(new If<T, T>(condition, onTrue, onFalse, z => z)).To();

    public static Free<Instruction<T>, T> ReadLine<T>() =>
        Free<Instruction<T>>.LiftF(new ReadLine<T, T>(t => t)).To();

    public static Free<Instruction<T>, Unit> WriteLine<T>(string line) =>
        Free<Instruction<T>>.LiftF(new WriteLine<T, Unit>(line, Unit.Instance)).To();

    public static Free<Instruction<T>, T> Const<T>(T value) =>
        Free<Instruction<T>>.Pure(value).To();
}

/// <summary>
/// Functor implementation 
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class Instruction<T> : FunctionalCSharp.New.IFunctor<Instruction<T>>
{
    public static IKind<Instruction<T>, VNext> Map<TNext, VNext>(IKind<Instruction<T>, TNext> f, Func<TNext, VNext> fun)
    {
        var ff = (Instruction<T, TNext>)f;
        return ff switch
        {
            Add<T, TNext> add => new Add<T, VNext>(add.Arg1, add.Arg2, t => fun(add.Next(t))),
            If<T, TNext> @if => new If<T, VNext>(@if.Condition,
                Free<Instruction<T>>.Map(@if.OnTrue, fun).To(),
                Free<Instruction<T>>.Map(@if.OnFalse, fun).To(),
                t => fun(@if.Next(t))
            ),
            Mul<T, TNext> mul => new Mul<T, VNext>(mul.Arg1, mul.Arg2, t => fun(mul.Next(t))),
            ReadLine<T, TNext> readLine => new ReadLine<T, VNext>(t => fun(readLine.Next(t))),
            WriteLine<T, TNext> writeLine => new WriteLine<T, VNext>(writeLine.Line, () => fun(writeLine.Next())),
            _ => throw new ArgumentOutOfRangeException(nameof(f))
        };
    }
}