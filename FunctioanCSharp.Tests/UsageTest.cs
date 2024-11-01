using System;
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
        var list = new New.Monads.List<int>();
        var traverse = List.Traverse(list, Some<int>.Of);
        var maybe = traverse.To();


        Validation<string, List, ValidationError> a =
            Validation.Failed<string, ValidationError>(new ValidationErrorSpecific());
        Validation<int, List, ValidationError> b = Validation.Ok<int, ValidationError>(1);


        var res = Validation<List, ValidationError>.Lift2((a, b)
            => new { a, b }, a, b).To();

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