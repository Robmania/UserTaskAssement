using DataBalk.Task.Api.Enums;

namespace DataBalk.Task.Api.Infrastructure.MediatR;

public static class Utils
{
    public static T WithDetails<T>(this T msg, HttpRequest request, EOperation operation = EOperation.NotSpecified)
        where T : IAppMessage
    {
        if (msg is AppCommand cmd && operation != EOperation.NotSpecified)
        {
            cmd.Operation = operation;
        }
        return msg;
    }
}
