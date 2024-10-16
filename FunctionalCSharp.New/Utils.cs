namespace FunctionalCSharp.New
{
    static class Utils
    {
        public static Func<T, T> Id<T>() => a => a;
    }
}
