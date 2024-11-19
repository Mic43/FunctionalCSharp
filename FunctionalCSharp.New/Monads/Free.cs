using FunctionalCSharp.New.Base;

namespace FunctionalCSharp.New.Monads;

/// <summary>
/// Free monad data type
/// </summary>
/// <typeparam name="TFunctor">Functor that free monads is constructed from</typeparam>
/// <typeparam name="T">Type of the result of the computation</typeparam>
public abstract record Free<TFunctor, T> : IKind<Free<TFunctor>, T> where TFunctor : IFunctor<TFunctor>
{
    /// <summary>
    /// Produces new instance with modified functor type 
    /// </summary>
    /// <param name="naturalTransformation">transformation between functors to used to produce new free monad instance</param>
    /// <typeparam name="TFunctorB">new functor type</typeparam>
    /// <returns>instance of the free monad with modified functor type, according to provided </returns>natural transformation between functors
    public IKind<Free<TFunctorB>, T> Hoist<TFunctorB>(INaturalTransformation<TFunctor, TFunctorB> naturalTransformation)
        where TFunctorB : IFunctor<TFunctorB>
    {
        return this switch
        {
            Pure<TFunctor, T>(var value) => Free<TFunctorB>.Pure(value),
            Roll<TFunctor, T>(var free) => new Roll<TFunctorB, T>(
                naturalTransformation.Transform(TFunctor.Map(free, next => next.Hoist(naturalTransformation).To()))),
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}

public sealed record Pure<TFunctor, T> : Free<TFunctor, T> where TFunctor : IFunctor<TFunctor>
{
    public T Value { get; }

    internal Pure(T value) => Value = value;

    public void Deconstruct(out T value) => value = Value;
}

public sealed record Roll<TFunctor, T>
    : Free<TFunctor, T> where TFunctor : IFunctor<TFunctor>
{
    public IKind<TFunctor, Free<TFunctor, T>> Free { get; }

    internal Roll(IKind<TFunctor, Free<TFunctor, T>> free) => Free = free;

    public void Deconstruct(out IKind<TFunctor, Free<TFunctor, T>> free) => free = Free;
}

/// <summary>
/// Free monad 'typeclass' implementation
/// </summary>
/// <typeparam name="TFunctor">Functor that free monads is to be constructed from</typeparam>
public abstract class Free<TFunctor> : IMonad<Free<TFunctor>> where TFunctor : IFunctor<TFunctor>
{
    public static IKind<Free<TFunctor>, V> Map<T, V>(IKind<Free<TFunctor>, T> f, Func<T, V> fun) =>
        IMonad<Free<TFunctor>>.Map(f, fun);

    public static IKind<Free<TFunctor>, V> Apply<T, V>(IKind<Free<TFunctor>, T> applicative,
        IKind<Free<TFunctor>, Func<T, V>> fun) =>
        IMonad<Free<TFunctor>>.Apply(applicative, fun);

    public static IKind<Free<TFunctor>, V> Bind<T, V>(IKind<Free<TFunctor>, T> monad,
        Func<T, IKind<Free<TFunctor>, V>> fun)
    {
        return monad.To() switch
        {
            Pure<TFunctor, T> (var value) => fun(value),
            Roll<TFunctor, T> (var free) =>
                new Roll<TFunctor, V>(TFunctor.Map(free, f => Bind(f, fun).To())),
            _ => throw new ArgumentOutOfRangeException(nameof(monad))
        };
    }

    public static IKind<Free<TFunctor>, T> Pure<T>(T value) => new Pure<TFunctor, T>(value);

    public static IKind<Free<TFunctor>, T> LiftF<T>(IKind<TFunctor, T> functor) =>
        new Roll<TFunctor, T>(TFunctor.Map(functor, t => Pure(t).To()));

    public static T Iter<T>(Func<IKind<TFunctor, T>, T> fun, IKind<Free<TFunctor>, T> free)
    {
        switch (free.To())
        {
            case Pure<TFunctor, T> (var value):
                return value;
            case Roll<TFunctor, T> (var f):
                var iter = Iter<T>;
                var curried = Utils.Curry(iter)(fun);
                return fun(TFunctor.Map(f, curried));
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public static IKind<Free<TFunctorB>, T> Hoist<T, TFunctorB>(INaturalTransformation<TFunctor, TFunctorB> fun,
        IKind<Free<TFunctor>, T> free) where TFunctorB : IFunctor<TFunctorB> =>
        free.To().Hoist(fun);
}

public static class FreeExt
{
    public static Free<TFunctor, T> To<TFunctor, T>(this IKind<Free<TFunctor>, T> kind)
        where TFunctor : IFunctor<TFunctor> =>
        (Free<TFunctor, T>)kind;

    public static Free<TFunctor, Z> SelectMany<TFunctor, T, V, Z>(this Free<TFunctor, T> free,
        Func<T, Free<TFunctor, V>> binder,
        Func<T, V, Z> projection) where TFunctor : IFunctor<TFunctor>
    {
        return Free<TFunctor>
            .Bind(free, t => binder(t).Select(v => projection(t, v))).To();
    }

    public static Free<TFunctor, V> Select<TFunctor, T, V>(this Free<TFunctor, T> free, Func<T, V> mapper)
        where TFunctor : IFunctor<TFunctor> =>
        Free<TFunctor>.Map(free, mapper).To();
}