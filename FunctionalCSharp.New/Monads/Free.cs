namespace FunctionalCSharp.New.Monads;

public abstract record Free<TFunctor, T> : IKind<Free<TFunctor>, T> where TFunctor : IFunctor<TFunctor>
{
    public IKind<Free<TFunctorB>, T> Hoist<TFunctorB>(INaturalTransformation<TFunctor, TFunctorB> fun)
        where TFunctorB : IFunctor<TFunctorB>
    {
        return this switch
        {
            Pure<TFunctor, T> pure => Free<TFunctorB>.Pure(pure.Value),
            Roll<TFunctor, T> roll => new Roll<TFunctorB, T>(
                fun.Transform(TFunctor.Map(roll.Free, next => next.Hoist(fun).To()))),
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
        switch (monad.To())
        {
            case Pure<TFunctor, T> (var value):
                return fun(value);
            case Roll<TFunctor, T> (var free):
            {
                var ff = (IKind<Free<TFunctor>, T> f) => Bind(f, fun).To();
                return new Roll<TFunctor, V>(TFunctor.Map(free, ff));
            }
            default:
                throw new ArgumentOutOfRangeException(nameof(monad));
        }
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