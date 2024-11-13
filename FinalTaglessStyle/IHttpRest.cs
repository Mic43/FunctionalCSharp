using FunctionalCSharp.New;

namespace FinalTaglessStyle;

public interface IHttpRest<THttpRest> : IMonad<THttpRest> where THttpRest : IMonad<THttpRest>
{
    static abstract IKind<THttpRest, byte[]> Get(string address);
    static abstract IKind<THttpRest, byte[]> Post(string address,byte[] content);
}
