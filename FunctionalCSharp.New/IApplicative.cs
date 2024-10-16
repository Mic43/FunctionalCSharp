namespace FunctionalCSharp.New
{
    public interface  IApplicative<TApplicative> : IFunctor<TApplicative> where TApplicative : IFunctor<TApplicative>
    {
        static abstract IKind<TApplicative, V> Apply<T, V>(IKind<TApplicative, T> applicative,
            IKind<TApplicative, Func<T, V>> fun);
        
    }
}
