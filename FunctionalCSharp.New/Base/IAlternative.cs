namespace FunctionalCSharp.New.Base;

public interface IAlternative<TAlternative> : IApplicative<TAlternative> where TAlternative : IAlternative<TAlternative>
{
    static abstract IKind<TAlternative, T> Append<T>(IKind<TAlternative, T> a, IKind<TAlternative, T> b);
    static abstract IKind<TAlternative, T> Empty<T>();
}