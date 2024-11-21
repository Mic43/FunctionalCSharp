using FunctionalCSharp.New;
using FunctionalCSharp.New.Base;
using FunctionalCSharp.New.Monads;
using FunctionalCSharp.New.Monads.Free;

namespace DSL.Logging.Interpreters;

public class ConsoleLogInterpreter<T> : FreeInterpreterBase<T, LogLang>
{
     public override TNext InterpretSingle<TNext>(IKind<LogLang, TNext> command)
    {
        var cmd = (LogCommand<TNext>)command;
        Console.WriteLine(cmd.Message);
        return cmd.Next();
    }
}