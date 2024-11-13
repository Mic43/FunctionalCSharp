namespace FunctionalCSharp.New;

public record Unit
{
    private Unit()
    {
    }
    public static Unit Instance() => new();
}