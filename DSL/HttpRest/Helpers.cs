using FunctionalCSharp.New.Monads;

namespace DSL.HttpRest;

static class Helpers
{
    public static Free<HttpRest, Task<HttpResponseMessage>> Get(string address)
        => Free<HttpRest>.LiftF(
            new GetCommand<Task<HttpResponseMessage>>(address, a => a)).To();

    public static Free<HttpRest, Task<HttpResponseMessage>> Post(string address, HttpContent httpContent)
        => Free<HttpRest>.LiftF(
            new PostCommand<Task<HttpResponseMessage>>(address, httpContent, a => a)).To();

    public static Free<HttpRest, Task<HttpResponseMessage>> Put(string address, HttpContent httpContent)
        => Free<HttpRest>.LiftF(
            new PutCommand<Task<HttpResponseMessage>>(address, httpContent, a => a)).To();

    public static Free<HttpRest, Task<HttpResponseMessage>> Delete(string address)
        => Free<HttpRest>.LiftF(
            new DeleteCommand<Task<HttpResponseMessage>>(address, a => a)).To();
}