using FunctionalCSharp.New.Monads;
using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace FunctionalCSharp.Tests
{
    public class MaybeTest
    {
        [Theory()]
        [InlineData(-10, 0)]
        [InlineData(0, 100)]
        [InlineData(1000, 10)]
        public void WhenOkMapCorrectlyChangesValue(int initValue, int addedValue)
        {

            var sut = new Some<int>(initValue);

            var z = Maybe.Pure(5);

            Func<int, int> mapper = v => v + addedValue;

            var res = Maybe.Map(sut, mapper);
            

            Assert.Equal(mapper(initValue), ((Some<int>)res).Value);
        }
    }
}
