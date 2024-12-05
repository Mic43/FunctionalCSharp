using System;
using FunctionalCSharp.New;
using FunctionalCSharp.New.Base;
using FunctionalCSharp.New.Monads;
using Xunit;

namespace FunctionalCSharp.Tests;

public class TraversableTest
{
    [Fact]
    public void SomeListTraverseTest()
    {
        var init = new List<int>(new[] { 1, 2, 3 });
        
        var traverse = List.Traverse(init, Maybe.Of);
        var actual = traverse.To();

        switch (actual)
        {
            case None<IKind<List,int>> :
                Assert.Fail("None");
                break;
            case Some<IKind<List,int>> some:
                Assert.Equal(some.Value.To().SourceList,init.SourceList);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(actual));
        }
    }
    
    [Fact]
    public void NoneListTraverseTest()
    {
        var init = new New.Monads.List<int>(new[] { 1, 2, 3 });

        var actual = List.Traverse(init, _ => Maybe.None<int>()).To();
        var expected = Maybe<IKind<List, int>>.None();
        
        Assert.Equal(expected,actual);

    }

}