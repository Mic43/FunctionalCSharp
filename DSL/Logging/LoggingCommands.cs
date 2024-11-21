using FunctionalCSharp.New;
using FunctionalCSharp.New.Base;

namespace DSL.Logging;

public record LogCommand<TNext>(string Message, Func<TNext> Next) : IKind<Log, TNext>;