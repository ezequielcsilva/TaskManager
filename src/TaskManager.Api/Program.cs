using Serilog;
using TaskManager.Api.Configuration;
using TaskManager.Api.Middlewares;
using TaskManager.Application;
using TaskManager.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, loggerConfig) =>
    loggerConfig.ReadFrom.Configuration(context.Configuration));

builder.Services.AddControllers();

//builder.Services.AddEndpointsApiExplorer();

builder.Services.AddApplication();

builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddSwaggerConfiguration();

var app = builder.Build();

app.UseCors("TaskManagerOrigins");

app.UseSwaggerConfiguration();

app.UseHttpsRedirection();

app.UseMiddleware<RequestContextLoggingMiddleware>();

app.UseSerilogRequestLogging();

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program
{ }