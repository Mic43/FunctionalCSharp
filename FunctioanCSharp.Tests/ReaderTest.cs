using System.Diagnostics;
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
}