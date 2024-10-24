namespace FunctionalCSharp.New.Monads;

public record ReaderT<TEnv, TMonad, T>(Func<TEnv, IKind<TMonad, T>> RunReaderT)
    : IKind<ReaderT<TEnv, TMonad>, T> where TMonad : IMonad<TMonad>;

public class ReaderT<TEnv, TMonad> : IMonad<ReaderT<TEnv, TMonad>> where TMonad : IMonad<TMonad>
{
    public static IKind<ReaderT<TEnv, TMonad>, V> Map<T, V>(IKind<ReaderT<TEnv, TMonad>, T> f, Func<T, V> fun) =>
        IMonad<ReaderT<TEnv, TMonad>>.Map(f, fun);

    public static IKind<ReaderT<TEnv, TMonad>, V> Apply<T, V>(IKind<ReaderT<TEnv, TMonad>, T> applicative,
        IKind<ReaderT<TEnv, TMonad>, Func<T, V>> fun) =>
        IMonad<ReaderT<TEnv, TMonad>>.Apply(applicative, fun);

    public static IKind<ReaderT<TEnv, TMonad>, V> Bind<T, V>(IKind<ReaderT<TEnv, TMonad>, T> monad,
        Func<T, IKind<ReaderT<TEnv, TMonad>, V>> fun)
    {
        var readerT = monad.To();
        return new ReaderT<TEnv, TMonad, V>(env =>
        {
            var resT = readerT.RunReaderT(env);
            return TMonad.Bind(resT, t => fun(t).To().RunReaderT(env));
        });
    }

    public static IKind<ReaderT<TEnv, TMonad>, T> Pure<T>(T value) =>
        new ReaderT<TEnv, TMonad, T>(_ => TMonad.Pure(value));

    public static IKind<ReaderT<TEnv, TMonad>, T> Lift<T>(IKind<TMonad, T> monad) =>
        new ReaderT<TEnv, TMonad, T>(_ => monad);
}

public static class ReaderTExt
{
    public static ReaderT<TEnv, TMonad, Z> SelectMany<T, V, Z, TMonad, TEnv>(this ReaderT<TEnv, TMonad, T> readerT,
        Func<T, ReaderT<TEnv, TMonad, V>> binder,
        Func<T, V, Z> projection) where TMonad : IMonad<TMonad>
    {
        return ReaderT<TEnv, TMonad>
            .Bind(readerT,
                t => ReaderT<TEnv, TMonad>.Bind(binder(t), v => ReaderT<TEnv, TMonad>.Pure(projection(t, v)))).To();
    }

    public static ReaderT<TEnv, TMonad, V> Select<T, V, TMonad, TEnv>(this ReaderT<TEnv, TMonad, T> readerT,
        Func<T, V> mapper)
        where TMonad : IMonad<TMonad>
    {
        return ReaderT<TEnv, TMonad>.Map(readerT, mapper).To();
    }

    public static ReaderT<TEnv, TMonad, T> To<TEnv, TMonad, T>(this IKind<ReaderT<TEnv, TMonad>, T> kind)
        where TMonad : IMonad<TMonad>
    {
        return (ReaderT<TEnv, TMonad, T>)kind;
    }
}

// public record ReaderTmp<TEnv, T> : ReaderT<TEnv, Identity, T>
// {
//     public ReaderTmp(Func<TEnv, T> runReader) : base(e => new Identity<T>(runReader(e)))
//     {
//     }
// }