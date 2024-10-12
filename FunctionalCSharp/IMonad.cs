﻿using FunctionalCSharp.Result;
using System;
using System.Collections.Generic;

namespace FunctionalCSharp
{
    public interface IMonad<T> : IApplicative<T>
    {
        // As in functor definition, c# type system cannot bound returned type to the same implementation as source monad type
        // it cant be just monad<v> it must be same monad as monad<t>
        // Second problem: no pure/unit functions
        IMonad<V> Bind<V>(Func<T, IMonad<V>> binder);
        }

    }

    // try to define pure/unit funct using abstract class in order to define
    // some generic versions of map/apply in terms of bind
    
    //abstract class Monad<T> : IMonad<T>
    //{      
    //    // pure/unit equivalent 
    //    protected Monad(T value) { }

    //    public abstract IMonad<V> Bind<V>(Func<T, IMonad<V>> binder);
    //    public IFunctor<V> Map<V>(Func<T, V> mapper)
    //    {
    //        // again we cant just create instance (equivalent of pure func) of the proper monad
    //        return Bind(t => new Monad<V> (mapper(t))); //???
    //    }
    //    // unfortunately the same is true about apply method
    //    public abstract IApplicative<V> Apply<V>(IApplicative<Func<T, V>> fun);
    //}
//}
