using System.Reflection;

namespace DataBalk.Task.Api.Infrastructure;

public class AssemblyReference
{
    public static readonly Assembly Assembly = typeof(AssemblyReference).Assembly;
}
