using System;
using System.Reflection.Metadata.Ecma335;

namespace FunctionalCSharp.Result
{
    public class Ok<T, TError> : Result<T, TError>
    {
        private readonly T _value;

        public T Value => _value;

        public Ok(T value)
        {
            _value = value;
        }

        //same goes to this specific case applyu implementation.
        public override IApplicative<V> Apply<V>(IApplicative<Func<T, V>> app)
        {
            return (IApplicative<V>)app.Map(f => f(_value));
        }

        public override IMonad<V> Bind<V>(Func<T, IMonad<V>> binder)
        {
            return binder(_value);
        }
    }
}