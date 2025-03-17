using Microsoft.EntityFrameworkCore;
using ms.rabbitmq.Consumer;
using ms.rabbitmq.Middlewares;
using ms.task.api.Middlewares;
using ms.user.api.Consumers;
using ms.user.api.Middlewares;
using ms.user.application.Queries;
using ms.user.domain.Interfaces;
using ms.user.infrastructure.Data;
using ms.user.infrastructure.Repositories;
using NLog;
using NLog.Web;

var logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();

try
{
    logger.Trace("Starting Users Microservice...");

    var builder = WebApplication.CreateBuilder(args);

    builder.Logging.ClearProviders();
    builder.Host.UseNLog();
    builder.Services.AddLogging();
    builder.Logging.AddFilter("Microsoft", Microsoft.Extensions.Logging.LogLevel.Debug);
    builder.Logging.AddFilter("System", Microsoft.Extensions.Logging.LogLevel.Debug);
    builder.Logging.AddFilter("Microsoft.EntityFrameworkCore", Microsoft.Extensions.Logging.LogLevel.Debug);


    builder.Services.AddSingleton<IConsumer, UsersConsumer>();

    builder.Services.AddDbContext<UserDbContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")), ServiceLifetime.Transient);

    builder.Services.AddTransient<IUserRepository, UserRepository>();

    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    builder.Services.AddMediatR(cfg =>
    {
        cfg.RegisterServicesFromAssemblies(typeof(GetAllUsersQuery).Assembly);
    });

    builder.Services.AddCors(opt =>
    {
        opt.AddPolicy("AllowGatewayOrigin", builder =>
        {
            builder.WithOrigins("https://localhost:8031").AllowAnyHeader().AllowAnyMethod().AllowCredentials();
        });
    });

    var app = builder.Build();

    if (app.Environment.IsDevelopment())
    {
        logger.Trace("App in dev mode");
        app.UseSwagger();
        app.UseSwaggerUI();
    }



    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        try
        {
            var context = services.GetRequiredService<UserDbContext>();
            context.Database.Migrate();
            logger.Info("Database migration completed succesfully");
        }
        catch (Exception ex)
        {
            logger.Error(ex, "Error applying migrations");
        }
    }

    app.UseMiddleware<RequestLoggingMiddleware>();
    app.UseMiddleware<ExceptionHandlingMiddleware>();

    app.UseRabbitConsumer();

    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllers();

    app.Map("/{*url}", context =>
    {
        context.Response.StatusCode = 404;
        context.Response.ContentType = "application/json";
        var response = new
        {
            message = "Endpoint not found",
            statusCode = 404
        };
        return context.Response.WriteAsJsonAsync(response);
    });

    logger.Trace("Application started sucessfully");
    app.Run();
}
catch (Exception ex)
{
    logger.Fatal(ex, "Critical Error on application startup");
    throw;
}
finally
{
    LogManager.Shutdown();
}
