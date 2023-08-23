using Castle.Core.Configuration;
using DataBalk.Task.Api.Data;
using Moq;
using Xunit.Abstractions;

namespace DataBalk.Task.Tests;
public abstract class TestFixture
{
    protected readonly ITestOutputHelper _log;
    protected readonly IConfiguration _configuration;
    protected Mock<TaskDbContext> _dbContextMock;

    protected TestFixture(ITestOutputHelper log)
    {
        _log = log;
        
        Log("Initializing Text Fixture");
        Log("=============");
    }

    protected void Log(string msg)
    {
        Console.WriteLine(msg);
        _log.WriteLine(msg);
    }
}
