namespace Example.Api.Shared;

public class Result<T> : Result
{
    public T? Value { get; set; }
    public int? TotalItems { get; set; }
    public int? PageNumber { get; set; }
    public int? PageSize { get; set; }

    public static Result SuccessPaginatedResult(T value, int totalItems, int pageNumber, int pageSize)
    {
        return new Result<T>
        {
            Value = value,
            TotalItems = totalItems,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }
}
