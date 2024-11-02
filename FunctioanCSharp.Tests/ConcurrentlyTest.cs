using System.Threading.Tasks;
using FunctionalCSharp.New;
using Xunit;

namespace FunctionalCSharp.Tests;

public class ConcurrentlyTest
{
    [Fact]
    public void Test1()
    {
        var concurrently = Concurrently.FromTask(ct =>
        {
            var t = Task.Delay(1000, ct);
            t.Wait(ct);
            return 1;
        });

        var concurrently2 = Concurrently.FromTask(ct =>
                            {
                                var t = Task.Delay(2000, ct);
                                t.Wait(ct);
                                return "aaaa";
                            })
                            + Concurrently.FromTask(ct =>
                            {
                                var t = Task.Delay(2500, ct);
                                t.Wait(ct);
                                return "bbaaa";
                            });

        var conc = Concurrently
            .Lift2((t, v) => t + " " + v, concurrently, concurrently2)
            .To();
        string result = conc.Run();
        Assert.Equal("1 aaaa", result);
    }
}