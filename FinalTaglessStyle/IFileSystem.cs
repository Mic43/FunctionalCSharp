using System.Net;
using FunctionalCSharp.New;
using FunctionalCSharp.New.Monads;

namespace FinalTaglessStyle;

interface IFileSystem<TFileSystem> : IMonad<TFileSystem> where TFileSystem : IMonad<TFileSystem>
{
    static abstract IKind<TFileSystem, Unit> CreateFile(string path);
    static abstract IKind<TFileSystem, byte[]> ReadFile(string path);
}
