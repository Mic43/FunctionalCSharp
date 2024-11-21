using FunctionalCSharp.New.Monads;
using FunctionalCSharp.New.Monads.Free;

namespace DSL.HttpRest;

static class Helpers
{
    public static Free<HttpRestLang, HttpResponseMessage> HttpGet(string address)
        => Free<HttpRestLang>.LiftF(
            new GetCommand<HttpResponseMessage>(address, a => a)).To();

    public static Free<HttpRestLang, HttpResponseMessage> HttpPost(string address, HttpContent httpContent)
        => Free<HttpRestLang>.LiftF(
            new PostCommand<HttpResponseMessage>(address, httpContent, a => a)).To();

    public static Free<HttpRestLang, HttpResponseMessage> HttpPut(string address, HttpContent httpContent)
        => Free<HttpRestLang>.LiftF(
            new PutCommand<HttpResponseMessage>(address, httpContent, a => a)).To();

    public static Free<HttpRestLang, HttpResponseMessage> HttpDelete(string address)
        => Free<HttpRestLang>.LiftF(
            new DeleteCommand<HttpResponseMessage>(address, a => a)).To();
    
}