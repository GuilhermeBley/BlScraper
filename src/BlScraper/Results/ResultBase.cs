namespace BlScraper.Results;

public class ResultBase
{
    private readonly object? _result;
    private readonly bool _isSuccess;
    public bool IsSuccess => _isSuccess;
    public virtual object? ResultBaseObj => _result;

    protected ResultBase(bool isSuccess, object? result)
    {
        _result = result;
        _isSuccess = isSuccess;
    }

    public static ResultBase GetSuccess(object? result = null)
    {
        return new ResultBase(true, result);
    }

    public static ResultBase GetWithError(object? result = null)
    {
        return new ResultBase(false, result);
    }
}

public class ResultBase<T> : ResultBase
{
    private T _result;
    public T Result => _result;
    public override object ResultBaseObj => _result!;

    protected ResultBase(bool isSuccess, object? resultBase, T result) : base(isSuccess, resultBase)
    {
        _result = result;
    }

    public static ResultBase<T> GetSuccess(T result)
    {
        return new ResultBase<T>(true, result, result);
    }

    public static ResultBase<T> GetWithError(T result)
    {
        return new ResultBase<T>(false, result, result);
    }
}