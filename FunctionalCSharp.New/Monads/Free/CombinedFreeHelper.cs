using FunctionalCSharp.New.Base;
namespace FunctionalCSharp.New.Monads.Free;

public static class CombinedFreeHelper
{
    public static Free<Coproduct<TLanguageA, TLanguageB>, TNext> ToCoproduct<TNext, TLanguageA, TLanguageB>(
        this Free<TLanguageA, TNext> program) where TLanguageA : IFunctor<TLanguageA>
        where TLanguageB : IFunctor<TLanguageB> =>
        program.Hoist(new LanguageATransform<TLanguageA, TLanguageB>()).To();

    public static Free<Coproduct<TLanguageA, TLanguageB>, TNext> ToCoproduct<TNext, TLanguageA, TLanguageB>(
        this Free<TLanguageB, TNext> program) where TLanguageA : IFunctor<TLanguageA>
        where TLanguageB : IFunctor<TLanguageB> =>
        program.Hoist(new LanguageBTransform<TLanguageA, TLanguageB>()).To();


    private class LanguageBTransform<TLanguageA, TLanguageB> : INaturalTransformation<TLanguageB,
        Coproduct<TLanguageA, TLanguageB>> where TLanguageA : IFunctor<TLanguageA>
        where TLanguageB : IFunctor<TLanguageB>
    {
        public IKind<Coproduct<TLanguageA, TLanguageB>, TNext> Transform<TNext>(
            IKind<TLanguageB, TNext> functor) =>
            Coproduct<TLanguageA, TLanguageB, TNext>.Right(functor);
    }

    private class
        LanguageATransform<TLanguageA, TLanguageB> : INaturalTransformation<TLanguageA,
        Coproduct<TLanguageA, TLanguageB>>
        where TLanguageA : IFunctor<TLanguageA> where TLanguageB : IFunctor<TLanguageB>
    {
        public IKind<Coproduct<TLanguageA, TLanguageB>, TNext> Transform<TNext>(
            IKind<TLanguageA, TNext> functor) =>
            Coproduct<TLanguageA, TLanguageB, TNext>.Left(functor);
    }
}