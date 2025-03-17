using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using NLog;
using NLog.Web;
using ms.auth.application.Commands;
using ms.auth.application.Services;
using ms.auth.domain.Interfaces;
using ms.auth.infrastructure.Data;
using ms.auth.infrastructure.Repositories;
using ms.rabbitmq.Producers;
using ms.auth.api.Middlewares;

var logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();


try
{
    logger.Trace("Starting Auth Microservice...");

    var builder = WebApplication.CreateBuilder(args);
    var privateKey = builder.Configuration.GetValue<string>("JWT:PrivateKey")!;

    builder.Logging.ClearProviders();
    builder.Host.UseNLog();
    builder.Services.AddLogging();
    builder.Logging.AddFilter("Microsoft", Microsoft.Extensions.Logging.LogLevel.Debug);
    builder.Logging.AddFilter("System", Microsoft.Extensions.Logging.LogLevel.Debug);
    builder.Logging.AddFilter("Microsoft.EntityFrameworkCore", Microsoft.Extensions.Logging.LogLevel.Debug);


    builder.Services.AddLocalization();

    builder.Services.AddDbContext<AuthAccountsDBContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

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

    builder.Services.AddScoped<IAuthRepository, AuthRepository>();
    builder.Services.AddScoped<ITokenService, TokenService>();
    builder.Services.AddScoped<IProducer, EventProducer>();


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
        cfg.RegisterServicesFromAssemblies(typeof(CreateAuthAccountCommand).Assembly);
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
            var context = services.GetRequiredService<AuthAccountsDBContext>();
            context.Database.Migrate();
        }
        catch (Exception ex)
        {
            logger.Error(ex, "Error appliying migrations");
        }
    }

    //app.UseHttpsRedirection();

    app.UseCors("AllowAnyOrigin");

    app.UseAuthorization();

    app.MapControllers();

    app.UseMiddleware<RequestLoggingMiddleware>();
    app.UseMiddleware<ExceptionHandlingMiddleware>();

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