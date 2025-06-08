using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CoffeeExpressAPI.Infrastructure.Data.Contexts;

namespace CoffeeExpressAPI.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestController : ControllerBase
    {
        private readonly CoffeeExpressDbContext _context;
        private readonly ILogger<TestController> _logger;

        public TestController(CoffeeExpressDbContext context, ILogger<TestController> logger)
        {
            _context = context;
            _logger = logger;
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
                    _logger.LogInformation("✅ Conexión a base de datos exitosa. Base de datos creada: {DatabaseCreated}\", databaseCreated");
                    
                    return Ok(new
                    {
                        Message = "✅ Conexión a base de datos exitosa",
                        DatabaseExists = !databaseCreated, // Si EnsureCreated retorna false, ya existía
                        DatabaseCreated = databaseCreated,
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
    }
}
