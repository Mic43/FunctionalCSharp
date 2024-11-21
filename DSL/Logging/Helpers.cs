using DSL.HttpRest;
using FunctionalCSharp.New;
using FunctionalCSharp.New.Monads.Free;

namespace DSL.Logging;

public static class Helpers
{
    public static Free<LogLang, T> Log<T>(string message) where T : new() => Free<LogLang>.LiftF(
        new LogCommand<T>(message, () => new T())).To();
}