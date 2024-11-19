using System.Net;
using System.Reflection.Metadata.Ecma335;
using FunctionalCSharp.New;
using FunctionalCSharp.New.Base;
using FunctionalCSharp.New.Monads;

namespace FinalTaglessStyle;

interface IFileSystem<T>
{
    static abstract IFileSystem<byte[]> CreateFile(string path);
    static abstract  IFileSystem<Unit> ReadFile(string path);
}

// class Tst : IFileSystem
// {
//     public static IFileSystem CreateFile(string path)
//     {
//         throw new NotImplementedException();
//     }
//
//     public static IFileSystem ReadFile(string path)
//     {
//         throw new NotImplementedException();
//     }
// }

interface IFileSystem2<TFileSystem> where TFileSystem : IMonad<TFileSystem>
{
    static abstract IKind<TFileSystem, Unit> CreateFile(string path);
    static abstract IKind<TFileSystem, byte[]> ReadFile(string path);
}

abstract class FileSystemMock<TMonad> : IFileSystem2<TMonad> where TMonad : IMonad<TMonad>
{
    public static IKind<TMonad, Unit> CreateFile(string path)
    {
        return TMonad.Pure(Unit.Instance());
    }

    public static IKind<TMonad, byte[]> ReadFile(string path)
    {
        return TMonad.Pure(Array.Empty<byte>());
    }
}