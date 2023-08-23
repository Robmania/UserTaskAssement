namespace DataBalk.Task.Api.Infrastructure.MediatR;

public interface IAppMessage
{
    public MessageMetaData MetaData { get; set; }
}
public class AppMessage: IAppMessage
{
    public MessageMetaData MetaData { get; set; } = new();
}

public class MessageMetaData
{
    public Guid MessageId { get; set; } = Guid.NewGuid();
    public Guid CorrelationId { get; set; } = Guid.NewGuid();
    public List<string> Logs { get; set; } = new();

}
