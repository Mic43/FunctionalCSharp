using System;
using FunctionalCSharp.Result;
using Xunit;

namespace FunctionalCSharp.Tests
{
    public class ResultTest
    {
        [Theory()]
        [InlineData(-10, 0)]
        [InlineData(0, 100)]
        [InlineData(1000, 10)]
        public void WhenOkMapCorrectlyChangesValue(int initValue, int addedValue)
        {
            var sut = Result<int, string>.Ok(initValue);

            Func<int, int> mapper = v => v + addedValue;
            Ok<int, string> res = (Ok<int, string>)sut.Map(mapper);


            Assert.Equal(mapper(initValue), res.Value);
        }

        [Theory()]
        [InlineData(-10, 0)]
        [InlineData(0, 100)]
        [InlineData(1000, 10)]
        public void SelectManyReturnsOkForBothOks(int initValue, int initValue2)
        {
            Result<int, string> m = Result<int, string>.Ok(initValue);
            Result<int, string> m2 = Result<int, string>.Ok(initValue2);

            Func<int, int, int> f = (a, b) => a + b;

            var l = from c in m
                    from c2 in m2
                    select f(c, c2);

            Assert.IsType<Ok<int, string>>(l);

        }
        [Theory()]
        [InlineData(-10, 0)]
        [InlineData(0, 100)]
        [InlineData(1000, 10)]
        public void SelectManyReturnsCorrectResultsForBothOks(int initValue, int initValue2)
        {
            Result<int, string> m = Result<int, string>.Ok(initValue);
            Result<int, string> m2 = Result<int, string>.Ok(initValue2);

            Func<int, int, int> f = (a, b) => a + b;
            var l = from c in m
                    from c2 in m2
                    select f(c, c2);

            Assert.Equal(f(initValue,initValue2), ((Ok<int, string>)l).Value);

        }
    }
}
