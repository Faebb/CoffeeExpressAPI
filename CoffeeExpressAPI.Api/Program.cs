using Microsoft.EntityFrameworkCore;
using CoffeeExpressAPI.Infrastructure.Data.Contexts;
using CoffeeExpressAPI.Application;
using CoffeeExpressAPI.Application.Mappings;
using CoffeeExpressAPI.Application.Validators;
using Serilog;

//Cofiguracion de Serilog
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

Log.Information("🚀 Iniciando CoffeeExpressAPI...");

try
{
    var builder = WebApplication.CreateBuilder(args);

    // Add services to the container.
    // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
    //builder.Services.AddOpenApi();

    //Cofigurar Serilog desde appsettings.json
    builder.Host.UseSerilog((context, services, configuration) => configuration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext());

    // ✅ Configurar Entity Framework según el entorno
    if (builder.Environment.IsEnvironment("Testing"))
    {
        // Para tests: usar InMemory database
        builder.Services.AddDbContext<CoffeeExpressDbContext>(options =>
            options.UseInMemoryDatabase("TestDatabase"));
        Log.Information("🧪 Usando InMemory database para testing");
    }
    else
    {
        // Para desarrollo/producción: usar SQL Server
        builder.Services.AddDbContext<CoffeeExpressDbContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
        Log.Information("🗃️ Usando SQL Server para {Environment}", builder.Environment.EnvironmentName);
    }

    // Configurar AutoMapper
    builder.Services.AddAutoMapperConfiguration();
    Log.Information("🗺️ AutoMapper configurado correctamente");

    // Configurar FluentValidation
    builder.Services.AddFluentValidationConfiguration();
    Log.Information("✅ FluentValidation configurado correctamente");

    // Configurar MediatR
    builder.Services.AddMediatR(cfg =>
    {
        cfg.RegisterServicesFromAssembly(AssemblyReference.Assembly);
    });

    // Agregar servicios al contenedor
    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    var app = builder.Build();

    //Middleware de Serilog para request Ligging
    app.UseSerilogRequestLogging(options =>
    {
        options.MessageTemplate = "🌐 HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms";
        options.GetLevel = (HttpContext, elapsed, ex) => ex != null
            ? Serilog.Events.LogEventLevel.Error
            : Serilog.Events.LogEventLevel.Information;
        options.EnrichDiagnosticContext = (diagnosticContext, HttpContent) =>
        {
            diagnosticContext.Set("RequestHots", HttpContent.Request.Host.Value);
            diagnosticContext.Set("RequestScheme", HttpContent.Request.Scheme);
        };
    });

    // Agregar middleware de Serilog para request logging (solo si no es testing)
    if (!app.Environment.IsEnvironment("Testing"))
    {
        app.UseSerilogRequestLogging(options =>
        {
            options.MessageTemplate = "🌐 HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms";
            options.GetLevel = (httpContext, elapsed, ex) => ex != null
                ? Serilog.Events.LogEventLevel.Error
                : Serilog.Events.LogEventLevel.Information;
            options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
            {
                diagnosticContext.Set("RequestHost", httpContext.Request.Host.Value);
                diagnosticContext.Set("RequestScheme", httpContext.Request.Scheme);
            };
        });
    }

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        //app.MapOpenApi();
        app.UseSwagger();
        app.UseSwaggerUI();
        Log.Information("📚 Swagger UI disponible en /swagger");
    }

    app.UseHttpsRedirection();
    app.UseAuthorization();
    app.MapControllers();

    Log.Information("✅ CoffeeExpressAPI configurado correctamente");
    app.Run();
}
catch (Exception ex)
{

    Log.Fatal(ex, "❌ Error fatal al iniciar CoffeeExpressAPI");
}
finally
{
    Log.CloseAndFlush();
}

public partial class Program { }