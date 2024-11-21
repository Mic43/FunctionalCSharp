using FunctionalCSharp.New.Base;

namespace DSL.Logging;

public abstract class LogLang : IFunctor<LogLang>
{
    public static IKind<LogLang, V> Map<T, V>(IKind<LogLang, T> f, Func<T, V> fun)
    {
        var logCommand = (LogCommand<T>)f;
        return new LogCommand<V>(logCommand.Message, () => fun(logCommand.Next()));
    }
}