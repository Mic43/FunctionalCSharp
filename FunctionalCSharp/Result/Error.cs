using System;

namespace FunctionalCSharp.Result
{
    public class Error<T, TError> : Result<T, TError>
    {
        private readonly TError _error;

        public Error(TError error)
        {
            _error = error;
        }

        public override IApplicative<V> Apply<V>(IApplicative<Func<T, V>> fun)
        {
            return Result<V, TError>.Error(_error); 
        }

        public override IMonad<V> Bind<V>(Func<T, IMonad<V>> binder)
        {
            return Result<V, TError>.Error(_error);
        }
    }
}