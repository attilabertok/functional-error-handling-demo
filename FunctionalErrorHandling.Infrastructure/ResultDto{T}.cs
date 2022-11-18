namespace FunctionalErrorHandling.Infrastructure;

public class ResultDto<T>
{
    internal ResultDto(T data) => (Succeeded, Data) = (true, data);

    internal ResultDto(IEnumerable<Error> error) => Errors = error;

    internal ResultDto(Error error) => Errors = new List<Error> {error};

    public bool Succeeded { get; }

    public bool Failed => !Succeeded;

    public T? Data { get; }

    public IEnumerable<Error> Errors { get; } = Enumerable.Empty<Error>();
}
