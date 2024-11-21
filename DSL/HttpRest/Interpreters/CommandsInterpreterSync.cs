using FunctionalCSharp.New.Base;
using FunctionalCSharp.New.Monads;
using FunctionalCSharp.New.Monads.Free;

namespace DSL.HttpRest.Interpreters;

class CommandsInterpreterSync : FreeInterpreterBase<HttpResponseMessage,HttpRest>, IDisposable
{
    private readonly HttpClient _httpClient = new();

    public override TNext InterpretSingle<TNext>(IKind<HttpRest, TNext> command)
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

// class CommandsInterpreterAsync
// {
//     public virtual Task<HttpRequestMessage> Interpret(Free<HttpRest, Task<HttpRequestMessage>> program)
//     {
//         switch (program)
//         {
//             case Pure<HttpRest, Task<HttpRequestMessage>>(var value):
//                 return value;
//             case Roll<HttpRest, Task<HttpRequestMessage>>(var free):
//                 var f = (HttpRestCommand<Free<HttpRest, Task<HttpRequestMessage>>>)free;
//                 switch (f)
//                 {
//                     case DeleteCommand<Free<HttpRest, Task<HttpRequestMessage>>> deleteCommand:
//                         Free<HttpRest, Task<HttpRequestMessage>>
//                         return Interpret(deleteCommand.Next());
//                         break;
//                     case GetCommand<Free<HttpRest, Task<HttpRequestMessage>>> getCommand:
//                         break;
//                     case PostCommand<Free<HttpRest, Task<HttpRequestMessage>>> postCommand:
//                         break;
//                     case PutCommand<Free<HttpRest, Task<HttpRequestMessage>>> putCommand:
//                         break;
//                     default:
//                         throw new ArgumentOutOfRangeException(nameof(f));
//                 }
//
//             default:
//                 throw new ArgumentOutOfRangeException(nameof(program));
//         }
//     }
// }