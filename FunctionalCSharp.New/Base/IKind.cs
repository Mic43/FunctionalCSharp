namespace FunctionalCSharp.New.Base;

public interface IKind<out F, A>;

public interface IKind<out A, B, C> : IKind<A, IKind<B, C>>;