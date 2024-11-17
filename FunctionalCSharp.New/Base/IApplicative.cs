namespace FunctionalCSharp.New.Base;

public interface IApplicative<TApplicative> : IFunctor<TApplicative> where TApplicative : IApplicative<TApplicative>
{
    static abstract IKind<TApplicative, V> Apply<T, V>(IKind<TApplicative, T> applicative,
        IKind<TApplicative, Func<T, V>> fun);

    static IKind<TApplicative, Z> Lift2<T, V, Z>(Func<T, V, Z> operation, IKind<TApplicative, T> app1,
        IKind<TApplicative, V> app2)
    {
        var res = TApplicative.Map<T, Func<V, Z>>(app1, t => v => operation(t, v));
        return TApplicative.Apply(app2, res);
    }
    
    public static abstract IKind<TApplicative, T> Pure<T>(T value);
}