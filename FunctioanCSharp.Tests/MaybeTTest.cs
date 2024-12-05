using System;
using FunctionalCSharp.New.Monads;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Xunit;

namespace FunctionalCSharp.Tests;

public class MaybeTTest
{
    [Fact]
    public void Tst()
    {
        Async<string> DoSthA() => Async<string>.FromResult("start");

        Async<Maybe<int>> DoSthB(string input) =>
            Async<Maybe<int>>.FromResult(input.StartsWith("start") ? Maybe.Of(1) : new None<int>());

        Async<string> DoSthC(int input) => Async<string>.FromResult(input > 0 ? "OK" : "ValidationError");


        var maybeT =
            from a in MaybeT<Async>.Lift(DoSthA()).To()
            from b in MaybeT<Async, int>.Of(DoSthB(a)).To()
            from c in MaybeT<Async>.Lift(DoSthC(b)).To()
            select c;

        var result = maybeT.RunMaybeT.To().Run();

        switch (result)
        {
            case None<string> :
                Assert.Fail("None");
                break;
            case Some<string> some:
                Assert.Equal("OK",some.Value);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(result));
        }
    }
}