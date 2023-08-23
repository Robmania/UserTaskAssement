namespace DataBalk.Task.Api.Infrastructure.MediatR;

public interface IResult
{
    bool IsSuccess { get; }
    IEnumerable<string> Failures { get; }
    void SetFailures(IEnumerable<string> failures);
}

public class Result : IResult
{
    public Result() { } //DO NOT remove this default constructor
    public Result(bool isSuccess, IEnumerable<string> failures)
    {
        IsSuccess = isSuccess;
        Failures = failures;
    }

    public bool IsSuccess { get; set; }
    public bool HasFailed => !IsSuccess;
    public IEnumerable<string> Failures { get; set; }

    public void SetFailures(IEnumerable<string> failures)
    {
        IsSuccess = false;
        Failures = failures;
    }

    public override string ToString()
    {
        return IsSuccess ? "Success" : $"Failure: {Environment.NewLine}{string.Join(", ", Failures)}";
    }
}

public class Result<T> : Result
{
    public Result() { } //DO NOT remove this default constructor
    public Result(bool isSuccess, IEnumerable<string> failures) : base(isSuccess, failures) { }
    public T Value { get; set; }
}

public class SuccessResult : Result
{
    public SuccessResult() : base(true, new List<string>()) { }
}

public class SuccessResult<T> : Result<T>
{
    public SuccessResult(T value) : base(true, new List<string>())
    {
        Value = value;
    }
}

public class FailureResult : Result
{
    public FailureResult(IEnumerable<string> failures) : base(false, failures) { }
    public FailureResult(string failure) : base(false, new[] { failure }) { }
}

public class FailureResult<T> : Result<T>
{
    public FailureResult(IEnumerable<string> failures) : base(false, failures)
    {
        Value = default!;
    }
    public FailureResult(string failure) : base(false, new[] { failure })
    {
        Value = default!;
    }
}
