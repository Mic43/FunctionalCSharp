using System;
using System.Numerics;
using System.Threading.Tasks;
using FunctionalCSharp.New;
using FunctionalCSharp.New.Monads;

namespace FunctionalCSharp.Tests;



public class Test
{
    class ValidationError
    {
    }

    class ValidationErrorSpecific : ValidationError
    {
    }

    public static void Tst()
    {
        // var list = new New.Monads.List<int>();
        // var traverse = List.Traverse(list, Some<int>.Of);
        // var maybe = traverse.To();
        //
        //
        // Validation<string, List, ValidationError> a =
        //     Validation.Failed<string, ValidationError>(new ValidationErrorSpecific());
        // Validation<int, List, ValidationError> b = Validation.Ok<int, ValidationError>(1);
        //
        //
        // var res = Validation<List, ValidationError>.Lift2((a, b)
        //     => new { a, b }, a, b).To();


        var concurrently = new Concurrently<int>(ct =>
        {
            var t = Task.Delay(1000, ct);
            t.Start();
            return 1;
        });

        var concurrently2 = new Concurrently<string>(ct =>
        {
            var t = Task.Delay(2000, ct);
            t.Start();
            return "aaaa";
        });

        var conc = Concurrently.Lift2((t, v) => t + " " + v, concurrently, concurrently2).To();
        string result = conc.Run();


        // switch (res)
        // {
        //     case ValidationFailed<, List, ValidationError> validationFailed:
        //         break;
        //     case ValidationOk<, List, ValidationError> validationOk:
        //         break;
        //     default:
        //         throw new ArgumentOutOfRangeException(nameof(res));
        // }

        // New.Monads.List.FoldAdd(list);
    }
}