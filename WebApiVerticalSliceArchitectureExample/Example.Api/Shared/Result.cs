namespace Example.Api.Shared;

public class Result
{
    public string[]? Errors { get; set; }
    public bool Success => Errors is null || Errors.Length == 0;

    public static Result SuccessResult()
    {
        return new Result();
    }

    public static Result<T> SuccessResult<T>(T value)
    {
        return new Result<T>
        {
            Value = value
        };
    }

    public static Result Failure(params string[] errors)
    {
        return new Result
        {
            Errors = errors
        };
    }
}
