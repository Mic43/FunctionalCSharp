namespace FunctionalCSharp.New.Monads;

public record Reader<TEnv, T> : ReaderT<TEnv, Identity, T>
{
    internal Reader(Func<TEnv, T> runReader) : base(e => new Identity<T>(runReader(e)))
    {
    }

    internal Reader(ReaderT<TEnv, Identity, T> readerT) : base(readerT.RunReaderT)
    {
    }

    public T RunReader(TEnv env) => RunReaderT(env).To().Value;
}

public abstract class Reader<TEnv>
{
    public static Reader<TEnv, TEnv> Ask() => new(ReaderT<TEnv, Identity>.Ask());

    public static Reader<TEnv, TEnvS> Asks<TEnvS>(Func<TEnv, TEnvS> f) => new(ReaderT<TEnv, Identity>.Asks(f));

    public static Reader<TEnv, T> Local<T>(Reader<TEnv, T> reader, Func<TEnv, TEnv> modifyEnvFunc) =>
        new(reader.Local(modifyEnvFunc));

    public static Reader<TEnv, T> Pure<T>(T value) => new(ReaderT<TEnv, Identity>.Pure(value).To());
}

public static class ReaderExt
{
    public static Reader<TEnv, Z> SelectMany<T, V, Z, TEnv>(this Reader<TEnv, T> reader,
        Func<T, Reader<TEnv, V>> binder, Func<T, V, Z> projection) =>
        new(ReaderTExt.SelectMany(reader, binder, projection));

    public static Reader<TEnv, V> Select<T, V, TEnv>(this Reader<TEnv, T> reader, Func<T, V> mapper) =>
        new(ReaderTExt.Select(reader, mapper));

    public static Reader<TEnv, T> To<TEnv, T>(this IKind<Reader<TEnv>, T> kind) => (Reader<TEnv, T>)kind;
}