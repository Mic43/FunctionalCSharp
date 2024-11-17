using FunctionalCSharp.New;
using FunctionalCSharp.New.Base;

namespace FinalTaglessStyle;

abstract class Combined<T, V, TMonad> : IFileSystem2<TMonad>, IHttpRest<TMonad>
    where T : IFileSystem2<TMonad> where TMonad : IMonad<TMonad> where V:IHttpRest<TMonad>
{
    public static IKind<TMonad, Unit> CreateFile(string path)
    {
        return T.CreateFile(path);
    }

    public static IKind<TMonad, byte[]> ReadFile(string path)
    {
        return T.ReadFile(path);
    }

    public static IKind<TMonad, byte[]> Get(string address)
    {
        return V.Get(address);
    }

    public static IKind<TMonad, byte[]> Post(string address, byte[] content)
    {
        return V.Post(address, content);
    }
}

// interface Z<T>
// {
//     public static abstract Z<T> Foo(Z<T> aaa);
// }
//
// class ZZ<T> : Z<T>
// {
//     public static Z<T> Foo()
//     {
//         return new ZZ<T>();
//     }
// }
//
// static class C
// {
//     public static void zz<V,T>() where V:Z<T>
//     { 
//         var z = V.Foo();
//     }
// }
//


