using FsCheck;
using FsCheck.Xunit;
using FunctionalCSharp.New.Monads;

namespace FunctionalCSharp.Tests;

public class WriterTTest
{
    [Property]
    public bool MaybeWriterTest(int input, string s)
    {
        Maybe<int> DoSthA(string input)
        {
            return string.IsNullOrEmpty(input) ? new None<int>() : Some<int>.Of(1);
        }

        Maybe<string> DoSthB(int input, string s)
        {
            return input < 0 ? new None<string>() : Some<string>.Of(s + "OK");
        }

        var res = 
            from a in WriterT<List, string, Maybe>.Lift(DoSthA(s)).To()
            from b in WriterT<List, string, Maybe>.Lift(DoSthB(a, s)).To()
            from c in WriterT<List, string, Maybe>.Writer(b,List.Pure("logged" + a)).To()
            select c;

        var maybe = res.RunWriterT.To();
        // Maybe.Map( maybe,res => res.)
        return true;
    }
}