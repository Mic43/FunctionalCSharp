using System.IO.Compression;
using FunctionalCSharp.New.Base;

namespace FunctionalCSharp.New.Monads;

public record ReaderT<TEnv, TMonad, T>
    : IKind<ReaderT<TEnv, TMonad>, T> where TMonad : IMonad<TMonad>
{
    internal Func<TEnv, IKind<TMonad, T>> Inner { get; }

    internal ReaderT(Func<TEnv, IKind<TMonad, T>> inner)
    {
        Inner = inner;
    }

    public ReaderT<TEnv, TMonad, T> Local(Func<TEnv, TEnv> modifyEnvFunc) =>
        new(env => Inner(modifyEnvFunc(env)));

    public IKind<TMonad, T> RunReaderT(TEnv env) =>
        Inner(env);
}

public abstract class ReaderT<TEnv, TMonad> : IMonad<ReaderT<TEnv, TMonad>>,
    IMonadTransformer<ReaderT<TEnv, TMonad>, TMonad>
    where TMonad : IMonad<TMonad>
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
            var resT = readerT.Inner(env);
            return TMonad.Bind(resT, t => fun(t).To().Inner(env));
        });
    }

    public static IKind<ReaderT<TEnv, TMonad>, T> Pure<T>(T value) =>
        new ReaderT<TEnv, TMonad, T>(_ => TMonad.Pure(value));

    public static IKind<ReaderT<TEnv, TMonad>, T> Lift<T>(IKind<TMonad, T> monad) =>
        new ReaderT<TEnv, TMonad, T>(_ => monad);

    public static ReaderT<TEnv, TMonad, TEnv> Ask() => new(TMonad.Pure);
    public static ReaderT<TEnv, TMonad, TEnvS> Asks<TEnvS>(Func<TEnv, TEnvS> f) => Map(Ask(), f).To();

    public static ReaderT<TEnv, TMonad, T> Local<T>(ReaderT<TEnv, TMonad, T> reader,
        Func<TEnv, TEnv> modifyEnvFunc) => reader.Local(modifyEnvFunc);

    public static IKind<TMonad, T> RunReaderT<T>(ReaderT<TEnv, TMonad, T> reader, TEnv env) =>
        reader.RunReaderT(env);
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