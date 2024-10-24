namespace FunctionalCSharp.New.Monads;

public sealed record Identity<T>(T Value) : IKind<Identity, T>;

public sealed class Identity : IMonad<Identity>
{
    public static IKind<Identity, V> Map<T, V>(IKind<Identity, T> f, Func<T, V> fun) => IMonad<Identity>.Map(f, fun);

    public static IKind<Identity, V> Apply<T, V>(IKind<Identity, T> applicative, IKind<Identity, Func<T, V>> fun) =>
        IMonad<Identity>.Apply(applicative, fun);

    public static IKind<Identity, V> Bind<T, V>(IKind<Identity, T> monad, Func<T, IKind<Identity, V>> fun) =>
        fun(monad.To().Value);

    public static IKind<Identity, T> Pure<T>(T value) => new Identity<T>(value);
}

public static class IdentityExt
{
    public static Identity<T> To<T>(this IKind<Identity, T> kind) => (Identity<T>)kind;
}