﻿// See https://aka.ms/new-console-template for more information

using DSL;
using DSL.HttpRest;
using DSL.HttpRest.Interpreters;
using DSL.Logging;
using DSL.Logging.Interpreters;
using FunctionalCSharp.New.Monads.Free;
using static DSL.HttpRest.Helpers;
using static DSL.Logging.Helpers;


var copyCommand = (string addressSource, string addressDestination) =>
    from response in HttpGet(addressSource)
    from res in HttpPost(addressDestination, response.Content)
    select res;


var copyCommandWithLog = (string addressSource, string addressDestination) =>
    from response in HttpGet(addressSource).ToCombined()
    from _ in Log<HttpResponseMessage>(response.ToString()).ToCombined()
    from res in HttpPost(addressDestination, response.Content).ToCombined()
    select res;

using CommandsInterpreterSync interpreter = new CommandsInterpreterSync();
var logInterpreter = new ConsoleLogInterpreter<HttpResponseMessage>();

var combinedInterpreter =
    new CombinedLanguageInterpreter<HttpResponseMessage, HttpRestLang, LogLang>(interpreter, logInterpreter);


var result = interpreter.Interpret(
    copyCommand("https://httpbin.org/get", "https://httpbin.org/post"));

var result2 = combinedInterpreter.Interpret(
    copyCommandWithLog("https://httpbin.org/get", "https://httpbin.org/post"));


Console.WriteLine(result2);