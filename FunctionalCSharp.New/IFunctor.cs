namespace FunctionalCSharp.New;

public interface IFunctor<TFunctor> where TFunctor : IFunctor<TFunctor>
{
    static abstract IKind<TFunctor, V> Map<T, V>(IKind<TFunctor, T> f, Func<T, V> fun);
}