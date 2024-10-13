using FunctionalCSharp.Result;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace FunctionalCSharp
{
    internal class Test
    {
        private void Foo()
        {
            Result<int, string> m = Result<int, string>.Ok(5);
            Result<int, string> m2 = Result<int, string>.Ok(5);

            var l = from c in m
                    from c2 in m2
                    select c + c2;
            var z = from c in m
                    select c;

            //Result<Result<int, string>, string> s;
            //Monads.Join<int>(s);

        }
    }
}
