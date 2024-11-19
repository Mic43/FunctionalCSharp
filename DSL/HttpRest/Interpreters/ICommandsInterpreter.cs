using System.Numerics;
using FunctionalCSharp.New.Base;
using FunctionalCSharp.New.Monads;

namespace DSL.HttpRest.Interpreters;

interface ICommandsInterpreter<TOutput>
{
    public TOutput Interpret(Free<HttpRest, TOutput> program);
    public TNext InterpretSingle<TNext>(IKind<HttpRest, TNext> command);
}

class CommandsInterpreterAsync : ICommandsInterpreter<Task<HttpResponseMessage>>, IDisposable
{
    private readonly HttpClient _httpClient = new();

    public Task<HttpResponseMessage> Interpret(Free<HttpRest, Task<HttpResponseMessage>> program)
    {
         return Free<HttpRest>.Iter(InterpretSingle, program);
    }

    public TNext InterpretSingle<TNext>(IKind<HttpRest, TNext> command)
    {
        return (HttpRestCommand<TNext>)command switch
        {
            DeleteCommand<TNext>(var address, var next) => 
                next(_httpClient.DeleteAsync(address)),
            GetCommand<TNext>(var address, var next) => 
                next(_httpClient.GetAsync(address)),
            PostCommand<TNext>(var address, var content, var next) => 
                next(_httpClient.PostAsync(address, content)),
            PutCommand<TNext>(var address, var content, var next) => 
                next(_httpClient.PutAsync(address, content)),
            _ => throw new ArgumentOutOfRangeException(nameof(command))
        };
    }

    public void Dispose()
    {
        _httpClient.Dispose();
    }
}