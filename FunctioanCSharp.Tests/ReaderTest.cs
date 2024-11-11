using System;
using System.Diagnostics;
using System.IO.Compression;
using FunctionalCSharp.New.Monads;
using Xunit;

namespace FunctionalCSharp.Tests;

public class ReaderTest
{
    [Fact]
    public void Tst()
    {
        Reader<int, string> GetInit(string s)
        {
            return Reader<int>.Pure(s);
        }

        var reader =
            from s in GetInit("start")
            from s2 in GetInit(s + "1")
            from env in Reader<int>.Ask()
            select new { Value = s2 + "2", Env = env + 1 };

        var actual = reader.RunReader(1);

        Assert.Equal(2, actual.Env);
        Assert.Equal("start12", actual.Value);
    }

    [Fact]
    public void ReaderTTest()
    {
        Maybe<int> DoSthA(string input)
        {
            return string.IsNullOrEmpty(input) ? new None<int>() : Some<int>.Of(1);
        }
        Maybe<string> DoSthB(int input,string s)
        {
            return input < 0  ? new None<string>() : Some<string>.Of(s+"OK");
        }

        float config = 5.0f;
        string input = "sss";
        
        var readerT = 
            from a in ReaderT<float, Maybe>.Lift(DoSthA(input)).To()
            from env in ReaderT<float, Maybe>.Ask()
            let i = a.ToString() + env
            from res in ReaderT<float, Maybe>.Lift(DoSthB(a, i)).To()
            select res;

        var actual = readerT.RunReaderT(config).To();
        
        switch (actual)
        {
            case None<string> :
                Assert.Fail("None");
                break;
            case Some<string> some:
                Assert.Equal("1" + config + "OK",some.Value);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(actual));
        }
        
    }
}