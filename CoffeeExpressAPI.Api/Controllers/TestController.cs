using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CoffeeExpressAPI.Infrastructure.Data.Contexts;
using CoffeeExpressAPI.Application.Mappings;
using CoffeeExpressAPI.Application.Validators;
using CoffeeExpressAPI.Application.Validators.Common;
using AutoMapper;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using static CoffeeExpressAPI.Application.Mappings.ApplicationMappingProfile;

namespace CoffeeExpressAPI.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestController : ControllerBase
    {
        private readonly CoffeeExpressDbContext _context;
        private readonly ILogger<TestController> _logger;
        private readonly IMapper _mapper;
        private readonly IValidator<TestSource> _validator;
        private readonly IServiceProvider _serviceProvider;

        public TestController(CoffeeExpressDbContext context, ILogger<TestController> logger, IMapper mapper, IValidator<TestSource> validator, IServiceProvider serviceProvider)
        {
            _context = context;
            _logger = logger;
            _mapper = mapper;
            _validator = validator;
            _serviceProvider = serviceProvider;
        }

        [HttpGet("database-connection")]
        public async Task<IActionResult> TestDatabaseConnection()
        {
            try
            {
                _logger.LogInformation("🔍 Probando conexión a base de datos...");

                // Intentar conectar a la base de datos
                var canConnect = await _context.Database.CanConnectAsync();

                if (canConnect)
                {
                    var databaseCreated = await _context.Database.EnsureCreatedAsync();
                    _logger.LogInformation("✅ Conexión a base de datos exitosa. Base de datos creada: {DatabaseCreated}", databaseCreated);

                    return Ok(new
                    {
                        Message = "✅ Conexión a base de datos exitosa",
                        DatabaseExists = !databaseCreated,
                        DatabaseCreated = databaseCreated,
                        DatabaseProvider = _context.Database.ProviderName,
                        Timestamp = DateTime.UtcNow
                    });
                }
                else
                {
                    _logger.LogError("❌ No se pudo conectar a la base de datos");
                    return BadRequest("❌ No se pudo conectar a la base de datos");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error al conectar con la base de datos");
                return StatusCode(500, new
                {
                    Message = "❌ Error al conectar con la base de datos",
                    Error = ex.Message
                });
            }
        }

        [HttpGet("serilog-test")]
        public IActionResult TestSerilog()
        {
            _logger.LogDebug("🐛 Debug log desde TestController");
            _logger.LogInformation("ℹ️ Information log desde TestController");
            _logger.LogWarning("⚠️ Warning log desde TestController");

            // Log estructurado con propiedades
            _logger.LogInformation("👤 Usuario {UserId} realizó acción {Action} en {Timestamp}",
                123, "TestSerilog", DateTime.UtcNow);

            return Ok(new
            {
                Message = "✅ Serilog configurado correctamente",
                LogLevels = new[] { "Debug", "Information", "Warning", "Error", "Fatal" },
                CheckConsole = "Revisa la consola para ver los logs",
                CheckFile = "Revisa la carpeta logs/ para ver los archivos de log"
            });
        }

        [HttpGet("automapper-test")]
        public IActionResult TestAutoMapper()
        {
            try
            {
                _logger.LogInformation("🗺️ Probando AutoMapper...");

                // Crear objeto de prueba para mapear
                var sourceObject = new Application.Mappings.ApplicationMappingProfile.TestSource
                {
                    Id = 1,
                    Name = "Café Espresso",
                    Price = 4.50m,
                    CreatedAt = DateTime.UtcNow
                };

                // Mapear a objeto destino
                var mappedObject = _mapper.Map<Application.Mappings.ApplicationMappingProfile.TestDestination>(sourceObject);

                _logger.LogInformation("✅ AutoMapper funcionando correctamente. Mapeado: {SourceName} -> {DestinationName}",
                    sourceObject.Name, mappedObject.ProductName);

                return Ok(new
                {
                    Message = "✅ AutoMapper configurado correctamente",
                    OriginalObject = sourceObject,
                    MappedObject = mappedObject,
                    ProfileCount = AutoMapperConfig.GetMappingProfiles().Count(),
                    Note = "Los mapeos de entidades se implementarán en Sprint 1.2"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error al probar AutoMapper");
                return StatusCode(500, new
                {
                    Message = "❌ Error al probar AutoMapper",
                    Error = ex.Message
                });
            }
        }

        [HttpPost("fluentvalidation-test")]
        public async Task<IActionResult> TestFluentValidation([FromBody] TestSource testObject)
        {
            try
            {
                _logger.LogInformation("🔍 Probando FluentValidation...");

                // Validar usando el validador inyectado
                var validationResult = await _validator.ValidateAsync(testObject);

                if (validationResult.IsValid)
                {
                    _logger.LogInformation("✅ Validación exitosa para objeto: {ObjectName}", testObject.Name);

                    return Ok(new
                    {
                        Message = "✅ FluentValidation configurado correctamente",
                        IsValid = true,
                        ValidatedObject = testObject,
                        ValidatorType = _validator.GetType().Name,
                        RegisteredValidators = FluentValidationConfig.GetValidatorTypes().Count()
                    });
                }
                else
                {
                    _logger.LogWarning("⚠️ Validación falló. Errores: {Errors}",
                        string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage)));

                    return BadRequest(new
                    {
                        Message = "❌ Validación falló",
                        IsValid = false,
                        Errors = validationResult.Errors.Select(e => new
                        {
                            Property = e.PropertyName,
                            Error = e.ErrorMessage,
                            AttemptedValue = e.AttemptedValue
                        }),
                        Note = "Esto demuestra que FluentValidation está funcionando correctamente"
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error al probar FluentValidation");
                return StatusCode(500, new
                {
                    Message = "❌ Error al probar FluentValidation",
                    Error = ex.Message
                });
            }
        }
    }


}
