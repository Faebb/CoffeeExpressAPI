using Microsoft.EntityFrameworkCore;
using CoffeeExpressAPI.Infrastructure.Data.Contexts;
using CoffeeExpressAPI.Application;
using CoffeeExpressAPI.Application.Mappings;
using CoffeeExpressAPI.Application.Validators;
using Serilog;

// ============================
// CONFIGURACIÓN DE LOGGING
// ============================

// Configuración de Serilog para la gestión de logs en la aplicación
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

Log.Information("🚀 Iniciando CoffeeExpressAPI...");

try
{
    var builder = WebApplication.CreateBuilder(args);

    // ============================
    // CONFIGURACIÓN DE LOGGING
    // ============================

    // Configurar Serilog desde appsettings.json para logs estructurados
    builder.Host.UseSerilog((context, services, configuration) => configuration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext());

    // ============================
    // CONFIGURACIÓN DE BASE DE DATOS
    // ============================

    // Configuración de Entity Framework según el entorno
    if (builder.Environment.IsEnvironment("Testing"))
    {
        // Para tests: usar base de datos en memoria
        builder.Services.AddDbContext<CoffeeExpressDbContext>(options =>
            options.UseInMemoryDatabase("TestDatabase"));
        Log.Information("🧪 Usando InMemory database para testing");
    }
    else
    {
        // Para desarrollo/producción: usar SQL Server con conexión configurada
        builder.Services.AddDbContext<CoffeeExpressDbContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
        Log.Information("🗃️ Usando SQL Server para {Environment}", builder.Environment.EnvironmentName);
    }

    // ============================
    // CONFIGURACIÓN DE SERVICIOS
    // ============================

    // Configuración de AutoMapper para mapeo de objetos
    builder.Services.AddAutoMapperConfiguration();
    Log.Information("🗺️ AutoMapper configurado correctamente");

    // Configuración de FluentValidation para validación de datos
    builder.Services.AddFluentValidationConfiguration();
    Log.Information("✅ FluentValidation configurado correctamente");

    // Configuración de MediatR para la gestión de eventos y comandos
    builder.Services.AddMediatR(cfg =>
    {
        cfg.RegisterServicesFromAssembly(AssemblyReference.Assembly);
    });

    // Agregar servicios de controladores y documentación de API
    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    var app = builder.Build();

    // ============================
    // CONFIGURACIÓN DE MIDDLEWARES
    // ============================

    // Middleware de Serilog para registro de solicitudes HTTP
    app.UseSerilogRequestLogging(options =>
    {
        options.MessageTemplate = "🌐 HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms";
        options.GetLevel = (HttpContext, elapsed, ex) => ex != null
            ? Serilog.Events.LogEventLevel.Error
            : Serilog.Events.LogEventLevel.Information;
        options.EnrichDiagnosticContext = (diagnosticContext, HttpContent) =>
        {
            diagnosticContext.Set("RequestHost", HttpContent.Request.Host.Value);
            diagnosticContext.Set("RequestScheme", HttpContent.Request.Scheme);
        };
    });

    // Agregar middleware de logging solo si no es entorno de testing
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

    // ============================
    // CONFIGURACIÓN DEL PIPELINE
    // ============================

    if (app.Environment.IsDevelopment())
    {
        // Habilitar Swagger en entorno de desarrollo
        app.UseSwagger();
        app.UseSwaggerUI();
        Log.Information("📚 Swagger UI disponible en /swagger");
    }

    app.UseHttpsRedirection(); // Habilitar redirección a HTTPS
    app.UseAuthorization(); // Habilitar autorización en la aplicación
    app.MapControllers(); // Mapear rutas a controladores

    Log.Information("✅ CoffeeExpressAPI configurado correctamente");
    app.Run(); // Ejecutar la aplicación
}
catch (Exception ex)
{
    // Manejo de errores fatales en la inicialización de la aplicación
    Log.Fatal(ex, "❌ Error fatal al iniciar CoffeeExpressAPI");
}
finally
{
    // Asegurar el cierre adecuado de Serilog
    Log.CloseAndFlush();
}

// Clase parcial para permitir pruebas en Program.cs
public partial class Program { }
