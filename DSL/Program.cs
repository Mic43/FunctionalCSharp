// See https://aka.ms/new-console-template for more information

using System.Security.AccessControl;
using DSL.HttpRest;
using DSL.HttpRest.Interpreters;
using DSL.Logging;
using DSL.Logging.Interpreters;
using FunctionalCSharp.New.Monads;
using FunctionalCSharp.New.Monads.Free;
using static DSL.HttpRest.Helpers;
using static DSL.Logging.Helpers;


var copyCommand = (string addressSource, string addressDestination) =>
    from response in HttpGet(addressSource)
    from res in HttpPost(addressDestination, response.Content)
    select res;


var copyCommandWithLog = (string addressSource, string addressDestination) =>
    from response in HttpGet(addressSource).ToCombined<HttpResponseMessage,HttpRest,Log>()
    from _ in Log<HttpResponseMessage>(response.ToString()).ToCombined<HttpResponseMessage,HttpRest,Log>()
    from res in HttpPost(addressDestination, response.Content).ToCombined<HttpResponseMessage,HttpRest,Log>()
    select res;

using CommandsInterpreterSync interpreter = new CommandsInterpreterSync();
var logInterpreter = new ConsoleLogInterpreter<HttpResponseMessage>();

var combinedInterpreter =
    new CombinedLanguageInterpreter<HttpResponseMessage, HttpRest, Log>(interpreter, logInterpreter);


var result = interpreter.Interpret(
    copyCommand("https://httpbin.org/get", "https://httpbin.org/post"));

var result2 = combinedInterpreter.Interpret(
    copyCommandWithLog("https://httpbin.org/get", "https://httpbin.org/post"));


Console.WriteLine(result2);