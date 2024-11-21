using FunctionalCSharp.New;
using FunctionalCSharp.New.Base;
using FunctionalCSharp.New.Monads;
using FunctionalCSharp.New.Monads.Free;

namespace FunctionalCSharp.SampleDSL.FirstLanguage;

// Sample DSL instructions
abstract record Instruction<T, TNext> : IKind<Instruction<T>, TNext>;

sealed record Add<T, TNext>(T Arg1, T Arg2, Func<T, TNext> Next)
    : Instruction<T, TNext>;

sealed record Mul<T, TNext>(T Arg1, T Arg2, Func<T, TNext> Next)
    : Instruction<T, TNext>;

sealed record Read<T, TNext>(Func<T, TNext> Next)
    : Instruction<T, TNext>;

sealed record Write<T, TNext>(T Value, Func<TNext> Next)
    : Instruction<T, TNext>;

sealed record If<T, TNext>(
    Func<bool> Condition,
    Free<Instruction<T>, TNext> OnTrue,
    Free<Instruction<T>, TNext> OnFalse,
    Func<T, TNext> Next)
    : Instruction<T, TNext>;

sealed record Try<T, TNext>(
    Free<Instruction<T>, TNext> Guarded,
    Free<Instruction<T>, TNext> OnCatch,
    Func<T, TNext> Next
) : Instruction<T, TNext>;

/// <summary>
/// Defers building AST of the provided program until interpretation
/// </summary>
/// <param name="Program"></param>
/// <param name="Next"></param>
/// <typeparam name="T"></typeparam>
/// <typeparam name="TNext"></typeparam>
sealed record Dynamic<T, TNext>(Func<Free<Instruction<T>, TNext>> Program, Func<T, TNext> Next)
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

    public static Free<Instruction<T>, T> Try<T>(
        Free<Instruction<T>, T> guarded,
        Free<Instruction<T>, T> onCatch) =>
        Free<Instruction<T>>.LiftF(new Try<T, T>(guarded, onCatch, z => z)).To();
    
    public static Free<Instruction<T>, T> Read<T>() =>
        Free<Instruction<T>>.LiftF(new Read<T, T>(t => t)).To();

    public static Free<Instruction<T>, Unit> Write<T>(T value) =>
        Free<Instruction<T>>.LiftF(new Write<T, Unit>(value, Unit.Instance)).To();

    public static Free<Instruction<T>, T> Const<T>(T value) =>
        Free<Instruction<T>>.Pure(value).To();

    public static Free<Instruction<T>, T> Dynamic<T>(Func<Free<Instruction<T>, T>> programGenerator) =>
        Free<Instruction<T>>.LiftF(new Dynamic<T, T>(programGenerator, _ => _)).To();
}

/// <summary>
/// Functor implementation 
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class Instruction<T> : IFunctor<Instruction<T>>
{
    public static IKind<Instruction<T>, VNext> Map<TNext, VNext>(IKind<Instruction<T>, TNext> f, Func<TNext, VNext> fun)
    {
        var ff = (Instruction<T, TNext>)f;
        return ff switch
        {
            Add<T, TNext> add => new Add<T, VNext>(add.Arg1, add.Arg2, add.Next.Compose(fun)),
            Dynamic<T, TNext> dynamic => new Dynamic<T, VNext>(
                () => Free<Instruction<T>>.Map(dynamic.Program(), fun).To(),
                dynamic.Next.Compose(fun)),
            If<T, TNext> @if => new If<T, VNext>(@if.Condition,
                Free<Instruction<T>>.Map(@if.OnTrue, fun).To(),
                Free<Instruction<T>>.Map(@if.OnFalse, fun).To(),
                @if.Next.Compose(fun)
            ),
            Mul<T, TNext> mul => new Mul<T, VNext>(mul.Arg1, mul.Arg2, mul.Next.Compose(fun)),
            Read<T, TNext> readLine => new Read<T, VNext>(readLine.Next.Compose(fun)),
            Try<T, TNext> @try => new Try<T, VNext>(
                Free<Instruction<T>>.Map(@try.Guarded, fun).To(),
                Free<Instruction<T>>.Map(@try.OnCatch, fun).To(),
                @try.Next.Compose(fun)),
            Write<T, TNext> writeLine => new Write<T, VNext>(writeLine.Value, () => fun(writeLine.Next())),
            _ => throw new ArgumentOutOfRangeException(nameof(f))
        };
    }
}