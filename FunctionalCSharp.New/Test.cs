using FunctionalCSharp.New.Monads;

namespace FunctionalCSharp.New;

public class Test
{
    public static void Tst()
    {
        var list = new Monads.List<int>();
        Monads.List.FoldAdd(list);
    }
}