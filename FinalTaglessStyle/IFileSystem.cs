using System.Net;
using FunctionalCSharp.New;
using FunctionalCSharp.New.Monads;

namespace FinalTaglessStyle;

// interface IFileSystem<TFileSystem> : IMonad<TFileSystem> where TFileSystem : IMonad<TFileSystem>
// {
//     static abstract IKind<TFileSystem, Unit> CreateFile(string path);
//     static abstract IKind<TFileSystem, byte[]> ReadFile(string path);
// }

interface IFileSystem2<TFileSystem> 
{
    static abstract IKind<TFileSystem, Unit> CreateFile(string path);
    static abstract IKind<TFileSystem, byte[]> ReadFile(string path);
}



record FileSystemMock<T>(Identity<T> Inner) : IKind<FileSystemMock, T>
{
    public FileSystemMock(T value) : this(Identity.Pure(value).To())
    {
    }
    public void Deconstruct(out Identity<T> inner)
    {
        inner = Inner;
    }
}

abstract class FileSystemMock : IFileSystem2<FileSystemMock>
{
    public static IKind<FileSystemMock, Unit> CreateFile(string path)
    {
        return new FileSystemMock<Unit>(Unit.Instance());
    }

    public static IKind<FileSystemMock, byte[]> ReadFile(string path)
    {
        return new FileSystemMock<byte[]>(Array.Empty<byte>());
    }
}
