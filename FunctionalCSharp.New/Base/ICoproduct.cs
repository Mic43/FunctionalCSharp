using FunctionalCSharp.New.Monads;

namespace FunctionalCSharp.New.Base;

public class Coproduct<TLeft, TRight, T> : IKind<Coproduct<TLeft, TRight>, T>
    where TLeft : IFunctor<TLeft> where TRight : IFunctor<TRight>
{
    public Result<IKind<TLeft, T>, IKind<TRight, T>> Get { get; }

    public V Match<V>(Func<IKind<TLeft, T>, V> onLeft, Func< IKind<TRight, T>, V> onRight)
        => Get.Either(onLeft, onRight);
    
    internal Coproduct(Result<IKind<TLeft, T>, IKind<TRight, T>> inner) => Get = inner;

    public static Coproduct<TLeft, TRight, T> Left(IKind<TLeft, T> left) =>
        new(new Ok<IKind<TLeft, T>, IKind<TRight, T>>(left));

    public static Coproduct<TLeft, TRight, T> Right(IKind<TRight, T> right) =>
        new(new Error<IKind<TLeft, T>, IKind<TRight, T>>(right));
}
/// <summary>
/// Coproduct of two functors is a functor too
/// </summary>
/// <typeparam name="TLeft"></typeparam>
/// <typeparam name="TRight"></typeparam>
public abstract class Coproduct<TLeft, TRight> : IFunctor<Coproduct<TLeft, TRight>>
    where TLeft : IFunctor<TLeft> where TRight : IFunctor<TRight>
{
    public static IKind<Coproduct<TLeft, TRight>, V> Map<T, V>(IKind<Coproduct<TLeft, TRight>, T> f, Func<T, V> fun)
    {
        return f.To().Get.Either(
            left => Coproduct<TLeft, TRight, V>.Left(TLeft.Map(left, fun)),
            right => Coproduct<TLeft, TRight, V>.Right(TRight.Map(right, fun)));
    }
}

public static class CoproductExt
{
    public static Coproduct<TLeft, TRight, T> To<TLeft, TRight, T>(this IKind<Coproduct<TLeft, TRight>, T> coProduct)
        where TLeft : IFunctor<TLeft> where TRight : IFunctor<TRight>
        => (Coproduct<TLeft, TRight, T>)coProduct;
}