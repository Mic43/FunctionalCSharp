using FunctionalCSharp.New.Base;

namespace FunctionalCSharp.New.Monads;

public record Writer<TMonoid, TOut, T> : WriterT<TMonoid, TOut, Identity, T> where TMonoid : IMonoid<TMonoid>
{
    internal Writer((IKind<TMonoid, TOut>, T) inner) : base(Identity.Pure(inner))
    {
    }

    internal Writer(IKind<Identity, (IKind<TMonoid, TOut>, T)> inner) : base(inner)
    {
    }

    public (IKind<TMonoid, TOut>, T) RunWriter => RunWriterT.To().Value;
}

public static class Writer<TMonoid, TOut> where TMonoid : IMonoid<TMonoid>
{
    public static Writer<TMonoid, TOut, T> Pure<T>(T value) =>
        new(WriterT<TMonoid, TOut, Identity>.Pure(value).To().RunWriterT);
}

public static class WriterExt
{
    public static Writer<TMonoid, TOut, Z> SelectMany<T, V, Z, TMonoid, TOut>(this Writer<TMonoid, TOut, T> reader,
        Func<T, Writer<TMonoid, TOut, V>> binder, Func<T, V, Z> projection) where TMonoid : IMonoid<TMonoid> =>
        new(WriterTExt.SelectMany(reader, binder, projection).RunWriterT);

    public static Writer<TMonoid, TOut, V> Select<T, V, TMonoid, TOut>(this Writer<TMonoid, TOut, T> reader,
        Func<T, V> mapper) where TMonoid : IMonoid<TMonoid> =>
        new(WriterTExt.Select(reader, mapper).RunWriterT);
}