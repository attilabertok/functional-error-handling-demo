using Unit = System.ValueTuple;

namespace FunctionalErrorHandling.Infrastructure;

public static class ActionExt
{
    public static Func<T, Unit> ToFunc<T>(this Action<T> action)
        => t => { action(t); return default; };

}