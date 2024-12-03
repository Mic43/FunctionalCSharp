# FunctionalCSharp

Library containing basic Haskell's typeclasses in C#: **Functor**, **Monad**, **Applicative**, **Foldable**, **Traversable** etc. and some of basic implementations like _Maybe_, _Result_ (Haskell's Either equivalent), _Reader_, _Writer_, simple _Async_ implementation etc.

Some most used monad transformers are included: _MaybeT_, _ReaderT_, _WriterT_, and _ListT_ (done right - eg. that can be used for basic streaming and obeys monad laws).

With this library You can easily create custom **DSL's** by using **Free** monad implementation. Custom languages can be easily combined using **Cofunctor** typeclass and natural transformation between functors.

Libary uses new C# language features, like static abstract interface methods, and recursive generic parameters.
Also, there is support for query syntax, which mimics (to some extent) Haskell do notation and F# computation expressions.


