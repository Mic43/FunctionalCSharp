using FunctionalCSharp.New.Base;

namespace DSL.HttpRest;

public abstract record HttpRestCommand<TNext>(string Address) : IKind<HttpRest, TNext>;

public record GetCommand<TNext>(string Address, Func<HttpResponseMessage, TNext> Next)
    : HttpRestCommand<TNext>(Address)
{
    public void Deconstruct(out string address, out Func<HttpResponseMessage, TNext> next)
    {
        address = Address;
        next = Next;
    }
}

public record DeleteCommand<TNext>(string Address, Func<HttpResponseMessage, TNext> Next)
    : HttpRestCommand<TNext>(Address)
{
    public void Deconstruct(out string address, out Func<HttpResponseMessage, TNext> next)
    {
        address = Address;
        next = Next;
    }
}

public record PostCommand<TNext>(string Address, HttpContent Content, Func<HttpResponseMessage, TNext> Next)
    : HttpRestCommand<TNext>(Address)
{
    public void Deconstruct(out string address, out HttpContent content,
        out Func<HttpResponseMessage, TNext> next)
    {
        address = Address;
        content = Content;
        next = Next;
    }
}

public record PutCommand<TNext>(string Address, HttpContent Content, Func<HttpResponseMessage, TNext> Next)
    : HttpRestCommand<TNext>(Address)
{
    public void Deconstruct(out string address, out HttpContent content,
        out Func<HttpResponseMessage, TNext> next)
    {
        address = Address;
        content = Content;
        next = Next;
    }
}