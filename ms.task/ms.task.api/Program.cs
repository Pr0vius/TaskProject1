using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Microsoft.EntityFrameworkCore;
using ms.task.api.Middlewares;
using ms.task.application.Queries;
using ms.task.domain.Interfaces;
using ms.task.infrastructure.Data;
using ms.task.infrastructure.Repositories;
using NLog.Web;
using NLog;
using System.Text;

var logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();

try
{
    logger.Trace("Stating Tasks Microservice...");
    var builder = WebApplication.CreateBuilder(args);

    var privateKey = builder.Configuration.GetValue<string>("JWT:PrivateKey")!;

    builder.Logging.ClearProviders();
    builder.Host.UseNLog();
    builder.Services.AddLogging();
    builder.Logging.AddFilter("Microsoft", Microsoft.Extensions.Logging.LogLevel.Debug);
    builder.Logging.AddFilter("System", Microsoft.Extensions.Logging.LogLevel.Debug);
    builder.Logging.AddFilter("Microsoft.EntityFrameworkCore", Microsoft.Extensions.Logging.LogLevel.Debug);



    builder.Services.AddDbContext<TasksDBContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

    builder.Services.AddScoped<ITaskRepository, TaskRepository>();


    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo { Title = "Auth Api Documentation", Version = "v1" });

        c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type = SecuritySchemeType.ApiKey,
            Scheme = "Bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Description = "Ingrese el token JWT en el siguiente formato: Bearer {token}"
        });

        c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
    });

    builder.Services.AddAuthentication(opt =>
    {
        opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    }).AddJwtBearer(opt =>
    {
        opt.RequireHttpsMetadata = false;
        opt.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = false,
            ValidateIssuer = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(privateKey)),
            ValidateLifetime = true,
            RequireExpirationTime = true,
            ClockSkew = TimeSpan.Zero
        };
    });

    builder.Services.AddMediatR(cfg =>
    {
        cfg.RegisterServicesFromAssemblies(typeof(GetAllTasksQuery).Assembly);
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
    app.UseMiddleware<RequestLoggingMiddleware>();
    app.UseMiddleware<ExceptionHandlingMiddleware>();



    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        try
        {
            var context = services.GetRequiredService<TasksDBContext>();
            context.Database.Migrate();
        }
        catch (Exception ex)
        {
            logger.Error(ex, "Error appliying the migrations");
        }
    }



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