using FunctionalCSharp.New.Monads;
using FunctionalCSharp.New.Monads.Free;

namespace DSL.HttpRest;

static class Helpers
{
    public static Free<HttpRest, HttpResponseMessage> HttpGet(string address)
        => Free<HttpRest>.LiftF(
            new GetCommand<HttpResponseMessage>(address, a => a)).To();

    public static Free<HttpRest, HttpResponseMessage> HttpPost(string address, HttpContent httpContent)
        => Free<HttpRest>.LiftF(
            new PostCommand<HttpResponseMessage>(address, httpContent, a => a)).To();

    public static Free<HttpRest, HttpResponseMessage> HttpPut(string address, HttpContent httpContent)
        => Free<HttpRest>.LiftF(
            new PutCommand<HttpResponseMessage>(address, httpContent, a => a)).To();

    public static Free<HttpRest, HttpResponseMessage> HttpDelete(string address)
        => Free<HttpRest>.LiftF(
            new DeleteCommand<HttpResponseMessage>(address, a => a)).To();
    
}