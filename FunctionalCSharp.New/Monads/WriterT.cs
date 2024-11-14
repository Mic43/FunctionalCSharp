namespace FunctionalCSharp.New.Monads;

/// <summary>
/// Writer monad transformer
/// </summary>
/// <typeparam name="TMonoid">type of the monoid to use ase the accumulator eg. List</typeparam>
/// <typeparam name="TOut">type of the accumulator type parameter</typeparam>
/// <typeparam name="TMonad">type of the source monad </typeparam>
/// <typeparam name="T">type of the monad's parameter</typeparam>
/// 
public record WriterT<TMonoid, TOut, TMonad, T>
    : IKind<WriterT<TMonoid, TOut, TMonad>, T> where TMonad : IMonad<TMonad>
    where TMonoid : IMonoid<TMonoid>
{
    public IKind<TMonad, (IKind<TMonoid, TOut>, T)> RunWriterT { get; }

    public IKind<TMonad, T> ExecWriterT => TMonad.Map(RunWriterT, tuple => tuple.Item2);

    internal WriterT(IKind<TMonad, (IKind<TMonoid, TOut>, T)> inner)
    {
        RunWriterT = inner;
    }

    public WriterT<TMonoid, TOut, TMonad, (T, V)> Listens<V>(Func<IKind<TMonoid, TOut>, V> fun) =>
        new(TMonad.Map(RunWriterT,
            tuple => (tuple.Item1, (tuple.Item2, fun(tuple.Item1)))));

    public WriterT<TMonoid, TOut, TMonad, (T, IKind<TMonoid, TOut>)> Listen() => Listens(
        o => o);

    public WriterT<TMonoid, TOut, TMonad, T> Censor(Func<IKind<TMonoid, TOut>, IKind<TMonoid, TOut>> fun) =>
        new(TMonad.Map(RunWriterT, tuple => (fun(tuple.Item1), tuple.Item2)));
}

public abstract class WriterT<TMonoid, TOut, TMonad> : IMonad<WriterT<TMonoid, TOut, TMonad>>
    where TMonad : IMonad<TMonad> where TMonoid : IMonoid<TMonoid>
{
    public static IKind<WriterT<TMonoid, TOut, TMonad>, V> Map<T, V>(IKind<WriterT<TMonoid, TOut, TMonad>, T> f,
        Func<T, V> fun) =>
        IMonad<WriterT<TMonoid, TOut, TMonad>>.Map(f, fun);

    public static IKind<WriterT<TMonoid, TOut, TMonad>, V> Apply<T, V>(
        IKind<WriterT<TMonoid, TOut, TMonad>, T> applicative, IKind<WriterT<TMonoid, TOut, TMonad>, Func<T, V>> fun) =>
        IMonad<WriterT<TMonoid, TOut, TMonad>>.Apply(applicative, fun);

    public static IKind<WriterT<TMonoid, TOut, TMonad>, V> Bind<T, V>(IKind<WriterT<TMonoid, TOut, TMonad>, T> monad,
        Func<T, IKind<WriterT<TMonoid, TOut, TMonad>, V>> fun)
    {
        var writerT = monad.To();

        var result = TMonad.Bind(writerT.RunWriterT,
            tuple =>
                TMonad.Bind(fun(tuple.Item2).To().RunWriterT,
                    tuple2 =>
                        TMonad.Pure((TMonoid.Combine(tuple.Item1, tuple2.Item1), tuple2.Item2))
                )
        );
        return new WriterT<TMonoid, TOut, TMonad, V>(result);
    }

    public static IKind<WriterT<TMonoid, TOut, TMonad>, T> Pure<T>(T value) =>
        new WriterT<TMonoid, TOut, TMonad, T>(TMonad.Pure((TMonoid.Identity<TOut>(), value)));

    public static IKind<WriterT<TMonoid, TOut, TMonad>, T> Lift<T>(IKind<TMonad, T> monad) =>
        new WriterT<TMonoid, TOut, TMonad, T>(TMonad.Map(monad, t => (TMonoid.Identity<TOut>(), t)));

    public static IKind<WriterT<TMonoid, TOut, TMonad>, T> Writer<T>(T value, IKind<TMonoid, TOut> outValue) =>
        new WriterT<TMonoid, TOut, TMonad, T>(TMonad.Pure((
            TMonoid.Combine(TMonoid.Identity<TOut>(), outValue), value)));

    public static IKind<WriterT<TMonoid, TOut, TMonad>, Unit> Tell(IKind<TMonoid, TOut> outValue) =>
        Writer(Unit.Instance(), outValue);

    public static IKind<WriterT<TMonoid, TOut, TMonad>, (T, V)> Listens<T, V>
        (IKind<WriterT<TMonoid, TOut, TMonad>, T> writerT, Func<IKind<TMonoid, TOut>, V> fun) =>
        writerT.To().Listens(fun);

    public static IKind<WriterT<TMonoid, TOut, TMonad>, (T, IKind<TMonoid, TOut>)> Listen<T>
        (IKind<WriterT<TMonoid, TOut, TMonad>, T> writerT) =>
        writerT.To().Listen();

    public static IKind<WriterT<TMonoid, TOut, TMonad>, T> Censor<T>(IKind<WriterT<TMonoid, TOut, TMonad>, T> writerT,
        Func<IKind<TMonoid, TOut>, IKind<TMonoid, TOut>> fun) =>
        writerT.To().Censor(fun);
}

public static class WriterTExt
{
    public static WriterT<TMonoid, TOut, TMonad, Z> SelectMany<T, V, Z, TMonoid, TOut, TMonad>(
        this WriterT<TMonoid, TOut, TMonad, T> writerT,
        Func<T, WriterT<TMonoid, TOut, TMonad, V>> binder,
        Func<T, V, Z> projection) where TMonad : IMonad<TMonad> where TMonoid : IMonoid<TMonoid> =>
        WriterT<TMonoid, TOut, TMonad>
            .Bind(writerT,
                t => binder(t).Select(v => projection(t,v))).To();

    public static WriterT<TMonoid, TOut, TMonad, V> Select<T, V, TMonoid, TOut, TMonad>(
        this WriterT<TMonoid, TOut, TMonad, T> writerT,
        Func<T, V> mapper)
        where TMonad : IMonad<TMonad> where TMonoid : IMonoid<TMonoid> =>
        WriterT<TMonoid, TOut, TMonad>.Map(writerT, mapper).To();

    public static WriterT<TMonoid, TOut, TMonad, T> To<TMonoid, TOut, TMonad, T>(
        this IKind<WriterT<TMonoid, TOut, TMonad>, T> kind)
        where TMonad : IMonad<TMonad> where TMonoid : IMonoid<TMonoid> =>
        (WriterT<TMonoid, TOut, TMonad, T>)kind;
}