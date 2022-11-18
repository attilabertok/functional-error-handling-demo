namespace FunctionalErrorHandling.Infrastructure;

public static class ResultDtoExtensions
{
    public static ResultDto<T> ToResult<T>(this Either<Error, T> either) =>
        either.Match(
            error => new ResultDto<T>(error),
            data => new ResultDto<T>(data));

    public static ResultDto<T> ToResult<T>(this Validation<T> validation) =>
        validation.IsValid
            ? new ResultDto<T>(validation.Value!)
            : new ResultDto<T>(validation.Errors);

    public static ResultDto<T> ToResult<T>(this Exceptional<T> exceptional, Action<Exception>? exceptionHandler = null) =>
        exceptional.Match(
            exception =>
            {
                exceptionHandler?.Invoke(exception);
                return new ResultDto<T>(exception.Message);
            },
            data => new ResultDto<T>(data));

    public static ResultDto<T> ToResult<T>(this Validation<Exceptional<T>> validation, Action<Exception>? exceptionHandler = null) =>
        validation.IsValid
            ? validation.Value.Match(
            exception =>
            {
                exceptionHandler?.Invoke(exception);
                return new ResultDto<T>(exception.Message);
            },
            data => new ResultDto<T>(data))
    : new ResultDto<T>(validation.Errors);
}
