using MediatR;
using System.Diagnostics;

namespace DataBalk.Task.Api.Infrastructure.MediatR.PipelineBehaviours;

public class LoggingPipelineBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : AppMessage
{
    private readonly ILogger<TRequest> logger;

    public LoggingPipelineBehaviour(ILogger<TRequest> logger)
    {
        this.logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var timer = new Stopwatch();
        timer.Start();

        var result = await next();

        timer.Stop();
        request.MetaData.Logs.Add($"Execution Milliseconds: {timer.ElapsedMilliseconds}");
        Log(request, result, timer.ElapsedMilliseconds);

        return result;
    }

    private void Log(TRequest req, TResponse res, long executionTimeInMilliseconds)
    {
        var obj = new LoggingMeta<TRequest>(req, executionTimeInMilliseconds);

        if (res is Result result)
        {
            obj.MessageSucceeded = result.IsSuccess;
            obj.MessageFailures = string.Join(", ", result.Failures);
        }
        logger.LogInformation("{@pipelineLoggingObject}", obj);
    }
}

public class LoggingMeta<T>
{
    public LoggingMeta(T message, long executionMilliseconds)
    {
        Message = message;
        ExecutionMilliseconds = executionMilliseconds;
    }

    public bool MessageSucceeded { get; set; } = true; // default to true

    public T Message { get; set; }

    public string MessageFailures { get; set; } = string.Empty;

    public long ExecutionMilliseconds { get; set; }
}
