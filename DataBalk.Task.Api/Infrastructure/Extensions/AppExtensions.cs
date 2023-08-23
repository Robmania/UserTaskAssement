using DataBalk.Task.Api.Infrastructure.MediatR.PipelineBehaviours;
using FluentValidation;
using MediatR;
using System.Reflection.Metadata;

namespace DataBalk.Task.Api.Infrastructure.Extensions;

public static class AppExtensions
{
    public static void ConfigureAppServices(this IServiceCollection services, AppSettings settings)
    {
        services.AddSingleton(settings);

        services.AddValidatorsFromAssembly(typeof(AssemblyReference).Assembly);
        services.AddMediatrToApp();
        services.AddAutoMapper(typeof(AssemblyReference).Assembly);
    }

    private static void AddMediatrToApp(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(typeof(AssemblyReference).Assembly));

        // MediatR Pipeline Behaviours - outer most handler at the top, inner most handler at the bottom
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ErrorHandlerPipelineBehaviour<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingPipelineBehaviour<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(FluentValidationPipelineBehaviour<,>));


        /*
         *  ErrorHandlerPipelineBehaviour (global generic error handling)
         *    |
         *    |_ calls: LoggingPipelineBehaviour (global generic logging)
         *          |
         *          |_ calls: FluentValidationPipelineBehaviour (validation logic for Commands)
         *                |
         *                |_ calls: Specific Command e.g. UpdateTaskCommand
         */
        // 
    }

}
