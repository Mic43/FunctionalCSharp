namespace FunctionalCSharp.New.Base;

public interface IMonadPlus<TMonad> : IMonad<TMonad>, IAlternative<TMonad>
    where TMonad : IMonad<TMonad>, IAlternative<TMonad>;