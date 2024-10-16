namespace FunctionalCSharp.New
{
    public interface IMonad<TMonad> : IApplicative<TMonad> where TMonad : IMonad<TMonad>
    {
        public static new IKind<TMonad, V> Map<T, V>(IKind<TMonad, T> f, Func<T, V> fun)
        {
            return TMonad.Bind(f, t => TMonad.Pure(fun(t)));
        }
        public static new IKind<TMonad, V> Apply<T, V>(IKind<TMonad, T> applicative, IKind<TMonad, Func<T, V>> fun)
        {
            return TMonad.Bind(applicative, t => TMonad.Map(fun, f => f(t)));
        }
        public static IKind<TMonad, T> Join<T>(IKind<TMonad, IKind<TMonad, T>> monad)
        {
            return TMonad.Bind(monad, innerMonad => innerMonad);
        }
        public static abstract IKind<TMonad, V> Bind<T, V>(IKind<TMonad, T> monad, Func<T, IKind<TMonad, V>> fun);

        public static abstract IKind<TMonad, T> Pure<T>(T value);

    }
}
