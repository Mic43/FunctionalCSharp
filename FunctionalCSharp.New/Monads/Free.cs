namespace FunctionalCSharp.New.Monads;

public abstract record Free<TFunctor, T> : IKind<Free<TFunctor>, T> where TFunctor : IFunctor<TFunctor>;

public sealed record Pure<TFunctor, T>(T Value) : Free<TFunctor, T> where TFunctor : IFunctor<TFunctor>;

public sealed record Roll<TFunctor, T>(IKind<TFunctor, Free<TFunctor, T>> Free)
    : Free<TFunctor, T> where TFunctor : IFunctor<TFunctor>;

public class Free<TFunctor> : IMonad<Free<TFunctor>> where TFunctor : IFunctor<TFunctor>
{
    public static IKind<Free<TFunctor>, V> Map<T, V>(IKind<Free<TFunctor>, T> f, Func<T, V> fun)
    {
        return IMonad<Free<TFunctor>>.Map(f, fun);
    }

    public static IKind<Free<TFunctor>, V> Apply<T, V>(IKind<Free<TFunctor>, T> applicative,
        IKind<Free<TFunctor>, Func<T, V>> fun)
    {
        return IMonad<Free<TFunctor>>.Apply(applicative, fun);
    }

    public static IKind<Free<TFunctor>, V> Bind<T, V>(IKind<Free<TFunctor>, T> monad,
        Func<T, IKind<Free<TFunctor>, V>> fun)
    {
        switch (monad.To())
        {
            case Pure<TFunctor, T> pure:
                return fun(pure.Value);
            case Roll<TFunctor, T> roll:
            {
                var ff = (IKind<Free<TFunctor>, T> f) => Bind(f, fun).To();
                return new Roll<TFunctor, V>(TFunctor.Map(roll.Free, ff));
            }
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public static IKind<Free<TFunctor>, T> Pure<T>(T value) => new Pure<TFunctor, T>(value);
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
            .Bind(free, t => Free<TFunctor>.Bind(binder(t), v => Free<TFunctor>.Pure(projection(t, v)))).To();
    }

    public static Free<TFunctor, V> Select<TFunctor, T, V>(this Free<TFunctor, T> maybe, Func<T, V> mapper)
        where TFunctor : IFunctor<TFunctor>
    {
        return Free<TFunctor>.Map(maybe, mapper).To();
    }
}