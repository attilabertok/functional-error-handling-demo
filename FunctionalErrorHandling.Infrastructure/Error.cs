namespace FunctionalErrorHandling.Infrastructure;

    public static partial class F
    {
        public static Error Error(string message) => new(message);
    }

    public record Error(string Message)
    {
        public override string ToString() => Message;

        public static implicit operator Error(string m) => new(m);
    }