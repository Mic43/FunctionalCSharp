using DSL.HttpRest;
using DSL.Logging;
using FunctionalCSharp.New.Base;
using FunctionalCSharp.New.Monads.Free;

namespace DSL;

public static class CombinedHelper
{
    public static Free<Coproduct<HttpRestLang, LogLang>, HttpResponseMessage> ToCombined(
        this Free<HttpRestLang, HttpResponseMessage> free) =>
        free.ToCoproduct<HttpResponseMessage, HttpRestLang, LogLang>();

    public static Free<Coproduct<HttpRestLang, LogLang>, HttpResponseMessage> ToCombined(
        this Free<LogLang, HttpResponseMessage> free) =>
        free.ToCoproduct<HttpResponseMessage, HttpRestLang, LogLang>();
}