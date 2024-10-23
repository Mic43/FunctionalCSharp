﻿namespace FunctionalCSharp.New;

public interface IAlternative<TAlternative> : IApplicative<TAlternative> where TAlternative : IAlternative<TAlternative>
{
    static abstract IKind<TAlternative, T> Append<T, V>(IKind<TAlternative, T> a, IKind<TAlternative, T> b);
    static abstract IKind<TAlternative, T> Empty<T>();
}