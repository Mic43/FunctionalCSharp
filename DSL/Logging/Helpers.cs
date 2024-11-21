using DSL.HttpRest;
using FunctionalCSharp.New;
using FunctionalCSharp.New.Monads.Free;

namespace DSL.Logging;

public static class Helpers
{
    public static Free<Log, T> Log<T>(string message) where T : new() => Free<Log>.LiftF(
        new LogCommand<T>(message, () => new T())).To();
}