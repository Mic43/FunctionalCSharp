using FunctionalCSharp.New;
using FunctionalCSharp.New.Base;
using FunctionalCSharp.New.Monads;
using FunctionalCSharp.New.Monads.Free;

namespace DSL.Logging.Interpreters;

public class ConsoleLogInterpreter<T> : FreeInterpreterBase<T, Log>
{
     public override TNext InterpretSingle<TNext>(IKind<Log, TNext> command)
    {
        var cmd = (LogCommand<TNext>)command;
        Console.WriteLine(cmd.Message);
        return cmd.Next();
    }
}