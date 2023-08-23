using System.Text;
using DataBalk.Task.Api.Data;
using DataBalk.Task.Api.Data.Repositories;
using DataBalk.Task.Api.Infrastructure.Extensions;
using DataBalk.Task.Api.Infrastructure.Services.TokenGenerator;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Exceptions;

namespace DataBalk.Task.Api.Infrastructure;

public static class TaskApiServicesRegistration
{
    public static void ConfigureSerilog(WebApplicationBuilder builder, AppSettings settings)
    {
        builder.Host.UseSerilog((ctx, services, lc) => lc
            .Enrich.FromLogContext()
            .Enrich.WithExceptionDetails()
            .Enrich.WithProperty("environment", settings.Environment)
            .Enrich.WithProperty("component", "API")
            .MinimumLevel.Debug()
            .WriteTo.Console());
    }

    public static void ConfigureServices(WebApplicationBuilder builder, AppSettings settings)
    {
        builder.Services.ConfigureAppServices(settings);

        builder.Services.AddDbContext<TaskDbContext>(options =>
            options.UseSqlite(builder.Configuration.GetConnectionString("dbconnectionstring")));

        builder.Services.AddScoped<ITokenGeneratorService, TokenGeneratorService>();
        builder.Services.AddScoped<ITaskRepository, TaskRepository>();
        builder.Services.AddScoped<IUserRepository, UserRepository>();

        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = builder.Configuration["Jwt:Issuer"],
                    ValidAudience = builder.Configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
                };
            });

        builder.Services.AddProblemDetails();
    }
}
