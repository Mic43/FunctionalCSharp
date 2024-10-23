using System.Diagnostics;
using FunctionalCSharp.New.Monads;
using Xunit;

namespace FunctionalCSharp.Tests;

public class ReaderTest
{
    [Fact]
    public void Tst()
    {
        Reader<int, string> GetInit()
        {
            return Reader<int>.Pure("start").To();
        }

        var reader =
            from s in GetInit()
            let r = GetInit()
            from env in r.Ask()
            select new { Value = s + "2", Env = env + 1 };

        var actual = reader.Run(1);

        Assert.Equal(2,actual.Env);
        Assert.Equal("start2", actual.Value);

    }
}