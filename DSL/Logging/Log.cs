using FunctionalCSharp.New.Base;

namespace DSL.Logging;

public abstract class Log : IFunctor<Log>
{
    public static IKind<Log, V> Map<T, V>(IKind<Log, T> f, Func<T, V> fun)
    {
        var logCommand = (LogCommand<T>)f;
        return new LogCommand<V>(logCommand.Message, () => fun(logCommand.Next()));
    }
}