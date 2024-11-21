using System.Diagnostics.CodeAnalysis;
using FunctionalCSharp.New;
using FunctionalCSharp.New.Base;

namespace DSL.HttpRest;

public abstract class HttpRestLang : IFunctor<HttpRestLang>
{
    public static IKind<HttpRestLang, V> Map<T, V>(IKind<HttpRestLang, T> f, Func<T, V> fun)
    {
        return (HttpRestCommand<T>)f switch
        {
            DeleteCommand<T>(var address, var next) => 
                new DeleteCommand<V>(address, next.Compose(fun)),
            GetCommand<T>(var address, var next) => 
                new GetCommand<V>(address, next.Compose(fun)),
            PostCommand<T>(var address, var content, var next) =>
                new PostCommand<V>(address, content, next.Compose(fun)),
            PutCommand<T>(var address, var content, var next) =>
                new PutCommand<V>(address, content, next.Compose(fun)),
            _ => throw new ArgumentOutOfRangeException(nameof(f))
        };
    }
}