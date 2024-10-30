using FunctionalCSharp.New;
using FunctionalCSharp.New.Monads;

namespace FunctionalCSharp.Tests;

public class Test
{
    public static void Tst()
    {
         var list = new New.Monads.List<int>();
         var traverse = List.Traverse(list,Some<int>.Of);
         var maybe = traverse.To();
             
                 
        // New.Monads.List.FoldAdd(list);

    }
}