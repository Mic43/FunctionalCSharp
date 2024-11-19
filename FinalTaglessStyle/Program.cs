// See https://aka.ms/new-console-template for more information


using FinalTaglessStyle;
using FunctionalCSharp.New;
using FunctionalCSharp.New.Base;
using FunctionalCSharp.New.Monads;


// interface IApp<TMonad> : IHttpRest<TMonad>, IFileSystem<TMonad> where TMonad : IMonad<TMonad>;


IKind<TMonad, byte[]> Foo<TDependency, TMonad>(string path, string address)
    where TDependency : IFileSystem2<TMonad>, IHttpRest<TMonad> where TMonad : IMonad<TMonad>
{
    var file = TDependency.ReadFile(path);
    var serverResult = TMonad.Bind(file, contents => TDependency.Post(address, contents));

    return serverResult;
}

IKind<Maybe, byte[]> Foo2<TDependency>(string path, string address)
    where TDependency : IFileSystem2<Maybe>, IHttpRest<Maybe>
{
    var file = TDependency.ReadFile(path);
    var serverResult = Maybe.Bind(file, contents => TDependency.Post(address, contents)).To();

    return serverResult;
}

var result = Foo<Combined<FileSystemMock<Maybe>, HttpRestMock<Maybe>, Maybe>, Maybe>("aaa", "bbbb");

var result2 = Foo2<Combined<FileSystemMock<Maybe>, HttpRestMock<Maybe>, Maybe>>("aaa", "bbb");

Console.WriteLine(result.To());

