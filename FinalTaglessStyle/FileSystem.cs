using FunctionalCSharp.New;
using FunctionalCSharp.New.Monads;

namespace FinalTaglessStyle;

record FileSystemI<T> : IKind<FileSystem, T>
{
    internal FileSystemI(Identity<T> Inner)
    {
        this.Inner = Inner;
    }

    internal Identity<T> Inner { get; init; }
}

static class Extensions
{
    public static FileSystemI<T> To<T>(this IKind<FileSystem, T> sys) => (FileSystemI<T>)sys;
}

abstract class FileSystem : IFileSystem<FileSystem>
{
    public static IKind<FileSystem, V> Map<T, V>(IKind<FileSystem, T> f, Func<T, V> fun) =>
        IFileSystem<FileSystem>.Map(f, fun);

    public static IKind<FileSystem, V>
        Apply<T, V>(IKind<FileSystem, T> applicative, IKind<FileSystem, Func<T, V>> fun) =>
        IFileSystem<FileSystem>.Apply(applicative, fun);

    public static IKind<FileSystem, V> Bind<T, V>(IKind<FileSystem, T> monad, Func<T, IKind<FileSystem, V>> fun)
    {
        return new FileSystemI<V>(Identity.Bind(monad.To().Inner, t => fun(t).To().Inner).To());
    }

    public static IKind<FileSystem, T> Pure<T>(T value) => new FileSystemI<T>(Identity.Pure(value).To());

    public static IKind<FileSystem, Unit> CreateFile(string path)
    {
        using var f = File.Create(path);
        return Pure(Unit.Instance());
    }

    public static IKind<FileSystem, byte[]> ReadFile(string path)
    {
        return Pure(File.ReadAllBytes(path));
    }
}