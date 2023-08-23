using MediatR;

namespace DataBalk.Task.Api.Infrastructure.MediatR.PipelineBehaviours;

public class ErrorHandlerPipelineBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : AppMessage
{
    private readonly ILogger<TRequest> logger;

    public ErrorHandlerPipelineBehaviour(ILogger<TRequest> logger)
    {
        this.logger = logger;
    }

    public async Task<TResponse> Handle(TRequest req, RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        try
        {
            return await next();
        }
        catch (Exception e)
        {
            var errMsg = e.GetBaseException().Message;

            req.MetaData.Logs.Add($"Error: {errMsg}");

            var errorLog = new LoggingMeta<TRequest>(req, 0)
            {
                MessageSucceeded = false,
                MessageFailures = errMsg
            };
            logger.LogError("{@error}", errorLog);
            logger.LogError(e, "Error for: {correlationId}", req.MetaData.CorrelationId);
            throw new UnhandledEventingPipelineException(e, req.MetaData.CorrelationId.ToString());
        }
    }
}

public class UnhandledEventingPipelineException : Exception
{
    public UnhandledEventingPipelineException(Exception ex, string correlationId)
        : base($"An unhandled error occurred. Please check the logs for error ID: {correlationId}", ex)
    {
        CorrelationId = correlationId;
    }

    public string CorrelationId { get; set; }
}
