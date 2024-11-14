using FunctionalCSharp.New;
using FunctionalCSharp.New.Monads;

namespace FinalTaglessStyle;

public interface IHttpRest<THttpRest>
{
    static abstract IKind<THttpRest, byte[]> Get(string address);
    static abstract IKind<THttpRest, byte[]> Post(string address, byte[] content);
}

record HttpRestMock<T>(Identity<T> Inner) : IKind<HttpRestMock, T>
{
    public HttpRestMock(T value) : this(Identity.Pure(value).To())
    {
    }
    public void Deconstruct(out Identity<T> inner)
    {
        inner = Inner;
    }
}

abstract class HttpRestMock : IHttpRest<HttpRestMock>
{
    public static IKind<HttpRestMock, byte[]> Get(string address)
    {
        return new HttpRestMock<byte[]>(Array.Empty<byte>());
    }

    public static IKind<HttpRestMock, byte[]> Post(string address, byte[] content)
    {
        return new HttpRestMock<byte[]>(Array.Empty<byte>());
    }
}