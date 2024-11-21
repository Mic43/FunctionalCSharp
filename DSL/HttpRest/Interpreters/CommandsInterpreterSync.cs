using FunctionalCSharp.New.Base;
using FunctionalCSharp.New.Monads;
using FunctionalCSharp.New.Monads.Free;
using FunctionalCSharp.New.Monads.Free.Interpreters;

namespace DSL.HttpRest.Interpreters;

class CommandsInterpreterSync<T> : FreeInterpreterBase<T,HttpRestLang>, IDisposable
{
    private readonly HttpClient _httpClient = new();

    public override TNext InterpretSingle<TNext>(IKind<HttpRestLang, TNext> command)
    {
        return (HttpRestCommand<TNext>)command switch
        {
            DeleteCommand<TNext>(var address, var next) =>
                next(_httpClient.DeleteAsync(address).GetAwaiter().GetResult()),
            GetCommand<TNext>(var address, var next) =>
                next(_httpClient.GetAsync(address).GetAwaiter().GetResult()),
            PostCommand<TNext>(var address, var content, var next) =>
                next(_httpClient.PostAsync(address, content).GetAwaiter().GetResult()),
            PutCommand<TNext>(var address, var content, var next) =>
                next(_httpClient.PutAsync(address, content).GetAwaiter().GetResult()),
            _ => throw new ArgumentOutOfRangeException(nameof(command))
        };
    }

    public void Dispose()
    {
        _httpClient.Dispose();
    }
}