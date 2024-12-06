using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using FunctionalCSharp.New.Monads;
using FunctionalCSharp.New.Monads.ListT;
using static FunctionalCSharp.New.Utils;

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
        // IEnumerable<string> GetAddresses()
        // {
        //     yield return @"http://yahoo.com";
        //     yield return @"http://bing.com";
        // }
        //
        // var client = new HttpClient();
        //
        // var list = new New.Monads.List<string>(GetAddresses());
        //
        // var res =
        //     from a in
        //         List.MSum(from c in list
        //             select ListT<Async>.Pure(c)).To()
        //     from b in ListT<Async>.Lift(
        //         client.GetStringAsync(a).ToMonad()
        //     ).To()
        //     select (a, b.Length);
        //
        // var x =
        //     from z in res
        //     select Log(z);
        // var async = x.RunListT;
        //
        // async.To().Run();

        var listT =
            from a in ListT<Async>.Repeat(5).Take(10)
            select a;

        var async = ListT<Async>.SplitAt(listT,15).To();
        var (list, rest) = async.Run();

        list.SourceList.ToList().ForEach(Console.WriteLine);
        
        var z =
            from a in rest
            select Log(a);
        Console.WriteLine();
        z.RunListT.To().Run();


        
        // var fold = listT.Fold(0,(a,b) => Async.Pure(a + b));
        // Console.WriteLine(fold.To().Run());



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


        // var concurrently = new Concurrently<int>(ct =>
        // {
        //     var t = Task.Delay(1000, ct);
        //     t.Start();
        //     return 1;
        // });
        //
        // var concurrently2 = new Concurrently<string>(ct =>
        // {
        //     var t = Task.Delay(2000, ct);
        //     t.Start();
        //     return "aaaa";
        // });
        //
        // var conc = Concurrently.Lift2((t, v) => t + " " + v, concurrently, concurrently2).To();
        // string result = conc.Run();


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