using Serilog;
using System.Reflection;
using DataBalk.Task.Api.Infrastructure;
using Microsoft.OpenApi.Models;
using Microsoft.Extensions.Options;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

var assemblyName = Assembly.GetExecutingAssembly().GetName().Name;

try
{
    Log.Information($"{assemblyName} Api starting up");


    var builder = WebApplication.CreateBuilder(args);
    Log.Information($"{assemblyName} - Retrieving application config settings: {nameof(AppSettings)} .");

    var settings = builder.Configuration.GetSection(nameof(AppSettings)).Get<AppSettings>();
    builder.Services.Configure<AppSettings>(builder.Configuration.GetSection(nameof(AppSettings)));

    // Add services to the container.
    TaskApiServicesRegistration.ConfigureSerilog(builder, settings);
    ConfigureServices(builder);
    TaskApiServicesRegistration.ConfigureServices(builder, settings);
    builder.Services.AddLogging();
    builder.Services.AddControllers();
    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("../swagger/v1/swagger.json", "DataBalk Task API V1");
            //c.RoutePrefix = string.Empty;
        });
        app.UseDeveloperExceptionPage();
    }

    app.UseHttpsRedirection();

    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllers();

    app.Map("/",
        () =>
            $"DataBalk Task API: {settings.Environment}, [{DateTime.UtcNow.ToLongTimeString()} UTC]");


    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, $"{assemblyName} API Startup Unhandled exception");
}
finally
{
    Log.Information($"{assemblyName} Api shut down complete");
    Log.CloseAndFlush();
}

void ConfigureServices(WebApplicationBuilder builder)
{
    builder.Services.Configure<RouteOptions>(options => options.LowercaseUrls = true);

    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo { Title = "DataBalk Task API", Version = "v1" });

        // Adding JWT Authentication
        var securitySchema = new OpenApiSecurityScheme
        {
            Description = "JWT Authorization header using the Bearer scheme.",
            Name = "Authorization",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            Reference = new OpenApiReference
            {
                Type = ReferenceType.SecurityScheme,
                Id = "Bearer"
            }
        };

        c.AddSecurityDefinition("Bearer", securitySchema);

        var securityRequirement = new OpenApiSecurityRequirement
        {
            { securitySchema, new[] { "Bearer" } }
        };

        c.AddSecurityRequirement(securityRequirement);
        // using System.Reflection;
        //var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        //c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
        });

}

namespace DataBalk.Task.Api
{
    public partial class Program { }
} // this is required to make the Program class visible to the tests, don't remove.