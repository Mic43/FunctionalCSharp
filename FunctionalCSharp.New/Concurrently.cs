using System.Numerics;

namespace FunctionalCSharp.New;

public record Concurrently<T>(Func<CancellationToken, T> Task) : IKind<Concurrently, T>,
    IAdditionOperators<Concurrently<T>, Concurrently<T>, Concurrently<T>>,
    IAdditiveIdentity<Concurrently<T>, Concurrently<T>>
{
    public T Run(CancellationToken cancellationToken) => Task(cancellationToken);
    public T Run() => Task(CancellationToken.None);

    public static Concurrently<T> operator +(Concurrently<T> left, Concurrently<T> right)
    {
        return Concurrently.Append(left, right).To();
    }

    public static Concurrently<T> AdditiveIdentity => Concurrently.Empty<T>().To();
}

public class Concurrently : IAlternative<Concurrently>
{
    public static IKind<Concurrently, V> Map<T, V>(IKind<Concurrently, T> f, Func<T, V> fun) =>
        new Concurrently<V>(ct => fun(f.To().Run()));

    public static IKind<Concurrently, V> Apply<T, V>(IKind<Concurrently, T> applicative,
        IKind<Concurrently, Func<T, V>> fun)
    {
        return new Concurrently<V>(
            ct =>
            {
                var task1 = Task.Factory.StartNew(() => applicative.To().Task(ct), ct);
                var task2 = Task.Factory.StartNew(() => fun.To().Task(ct), ct);

                var completed = Task.WhenAll(task1, task2);
                completed.Wait(ct);

                return task2.Result(task1.Result);
            }
        );
    }

    public static IKind<Concurrently, Z> Lift2<T, V, Z>(Func<T, V, Z> operation, IKind<Concurrently, T> app1,
        IKind<Concurrently, V> app2) =>
        IApplicative<Concurrently>.Lift2(operation, app1, app2);

    public static IKind<Concurrently, T> Pure<T>(T value) => new Concurrently<T>(_ => value);

    public static IKind<Concurrently, T> Append<T>(IKind<Concurrently, T> a, IKind<Concurrently, T> b)
    {
        return new Concurrently<T>(ct =>
        {
            using CancellationTokenSource cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(ct);
            var cancellationToken = cancellationTokenSource.Token;

            var task1 = Task.Factory.StartNew(() => a.To().Task(cancellationToken),
                cancellationToken);
            var task2 = Task.Factory.StartNew(() => b.To().Task(cancellationToken),
                cancellationToken);

            var completed = Task.WhenAny(task1, task2);
            completed.Wait(ct);

            cancellationTokenSource.Cancel();

            return completed.Result.Result;
        });
    }

    public static IKind<Concurrently, T> Empty<T>()
    {
        return new Concurrently<T>(ct =>
            {
                var task = Task.Delay(Timeout.InfiniteTimeSpan, ct);
                task.Wait(ct);
                return (T)new object();
            }
        );
    }
}

public static class ConcurrentlyExt
{
    public static Concurrently<T> To<T>(this IKind<Concurrently, T> kind) => (Concurrently<T>)kind;
}