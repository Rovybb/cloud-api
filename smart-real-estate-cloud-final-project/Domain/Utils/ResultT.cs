namespace Domain.Utils;
public class Result<T> : Result
{
    public T Data { get; set; }

    protected Result(bool isSuccess, T data, string errorMessage)
        : base(isSuccess, errorMessage)
    {
        Data = data;
    }

    public static Result<T> Success(T data)
    {
        return new Result<T>(true, data, "");
    }

    public static new Result<T> Failure(string errorMessage = "Unknown error occured!")
    {
        return new Result<T>(false, default!, errorMessage);
    }
}


