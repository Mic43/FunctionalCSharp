namespace FunctionalCSharp.New
{
    static class Utils
    {
        public static Func<T, T> Id<T>() => a => a;
    }

    interface Ass
    {
        static abstract void Cipa();
    }
    interface Ass2 : Ass
    {
       
    }
}
