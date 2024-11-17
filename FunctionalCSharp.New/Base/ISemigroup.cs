namespace FunctionalCSharp.New.Base;

public interface ISemigroup<TSemigroup> where TSemigroup : ISemigroup<TSemigroup>
{
    public static abstract IKind<TSemigroup, T> Combine<T>(IKind<TSemigroup, T> v1, IKind<TSemigroup, T> v2);
}