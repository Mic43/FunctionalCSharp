﻿using System.Numerics;

namespace FunctionalCSharp.New;

public interface IFoldable<TFoldable> where TFoldable : IFoldable<TFoldable>
{
    public static abstract TResult Fold<T, TResult>(IKind<TFoldable, T> foldable, TResult identity,
        Func<TResult,T, TResult> folder);
}