using System;

namespace FunctionalCSharp.Result
{
    public static class ResultHelper
    {
        public static Result<T, TError> Pure<T, TError>(T value)
        {
            return Result<T, TError>.Ok(value);
        }
    }
    public abstract class Result<T, TError> : Monad<T>
    {
        public static Ok<T, TError> Ok(T value)
        {
            return new(value);
        }
        public static Error<T, TError> Error(TError value)
        {
            return new(value);
        }

        //// we can define map in terms of bind now, for specific monad instance
        //// so similar code needs to be repeated for all specific monads
        //public IFunctor<V> Map<V>(Func<T, V> mapper)
        //{
        //    return Bind(t => ResultHelper.Pure<V, TError>(mapper(t)));
        //}

        // now we need applicative parameter to be the same type as our instance (so it needs to be monad too)
        // But the compiler cant check this at compile time, so,unfortunately, we cant define apply in terms of bind
        // amd we must leave implementation empty
        //public abstract IApplicative<V> Apply<V>(IApplicative<Func<T, V>> applicative);
        //{

        //    //return Bind(t => )
        //}

        //public abstract IMonad<V> Bind<V>(Func<T, IMonad<V>> binder);

        public override IMonad<V> Pure<V>(V value)
        {
            return ResultHelper.Pure<V,TError>(value);
        }
    }
    /// <summary>
    /// Some helper functions for query comprehensions to work
    /// </summary>
    public static class ResultExt
    {
        public static Result<Z, TError> SelectMany<T, V, Z, TError>(this Result<T, TError> f, Func<T, Result<V, TError>> binder,
            Func<T, V, Z> projection)
        {
            return (Result<Z, TError>)f.Bind(t => binder(t).Bind(v => ResultHelper.Pure<Z, TError>(projection(t, v))));
        }
        public static Result<V, TError> Select<T,V,TError>(this Result<T,TError> f,Func<T, V> mapper)
        {
            return (Result<V, TError>)f.Map(mapper);
        }
    }
}