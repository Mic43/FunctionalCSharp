using FunctionalCSharp.New;
using FunctionalCSharp.New.Base;
using FunctionalCSharp.New.Monads;

namespace FinalTaglessStyle;

public interface IHttpRest<THttpRest> where THttpRest:IMonad<THttpRest>
{
    static abstract IKind<THttpRest, byte[]> Get(string address);
    static abstract IKind<THttpRest, byte[]> Post(string address, byte[] content);
}

// record HttpRestMock<T>(Identity<T> Inner) : IKind<HttpRestMock, T>
// {
//     public HttpRestMock(T value) : this(Identity.Pure(value).To())
//     {
//     }
//     public void Deconstruct(out Identity<T> inner)
//     {
//         inner = Inner;
//     }
// }

abstract class HttpRestMock<TMonad> : IHttpRest<TMonad> where TMonad : IMonad<TMonad>
{
    public static IKind<TMonad, byte[]> Get(string address)
    {
        return TMonad.Pure(Array.Empty<byte>());
    }

    public static IKind<TMonad, byte[]> Post(string address, byte[] content)
    {
        return  TMonad.Pure(Array.Empty<byte>());
    }
}