using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Numerics;
using System.Threading.Channels;
using System.Threading.Tasks;
using FunctionalCSharp.New;
using FunctionalCSharp.New.Applicatives;
using FunctionalCSharp.New.Monads;
using FunctionalCSharp.New.Monads.ListT;
using Microsoft.FSharp.Core;
using Unit = FunctionalCSharp.New.Unit;

namespace FunctionalCSharp.Tests;

interface IInterface
{
    static virtual int Z() => 1;
}

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
        IEnumerable<string> GetAddresses()
        {
            yield return @"http://yahoo.com";
            yield return @"http://bing.com";
        }

        //
        // var a = ListT<Async>.Append(
        //     ListT<Async>.Pure("aaaaa"),
        //     ListT<Async>.Pure("bbbbb")).To();
        // var r = 
        //     from x in a
        //     select Utils.Log(x);
        //
        // r.Run().To().Run();

        //
        var client = new HttpClient();
        //
        var list = new New.Monads.List<string>(GetAddresses());
        var list1 = from c in list
            select ListT<Async>.Pure(c);
        
        var mSum = List.MSum(list1).To();
        //
        // var x = 
        //     from a in mSum
        //     select Utils.Log(a);
        //
        // x.Run().To().Run();

        var res =
            from a in mSum
            // from b in ListT<Async>.Lift(Task.Run(() => client.GetStringAsync(a)).ToAsync()).To()
            from b in ListT<Async>.Lift(
                client.GetStringAsync(a).ToAsync()
            ).To()
            select (a, b.Length);
        
        var x = from z in res
            select Utils.Log(z);
        var async = x.Run();
        
        async.To().Run();


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