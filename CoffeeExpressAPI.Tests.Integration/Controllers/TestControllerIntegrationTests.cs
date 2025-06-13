using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using FluentAssertions;
using System.Net;
using System.Text;
using System.Text.Json;
using CoffeeExpressAPI.Infrastructure.Data.Contexts;
using Microsoft.AspNetCore.Hosting;

namespace CoffeeExpressAPI.Tests.Integration.Controllers;

public class TestControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>, IDisposable
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;
    private readonly string _testDatabaseName;

    public TestControllerIntegrationTests(WebApplicationFactory<Program> factory)
    {
        // Crear un nombre único para la base de datos de prueba
        _testDatabaseName = $"CoffeeExpressTestDB_{Guid.NewGuid():N}";

        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                // Reemplazar la configuración de base de datos
                var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<CoffeeExpressDbContext>));
                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                // Usar SQL Server con base de datos temporal para testing
                services.AddDbContext<CoffeeExpressDbContext>(options =>
                {
                    options.UseSqlServer($"Server=.\\SQLEXPRESS;Database={_testDatabaseName};Integrated Security=true;TrustServerCertificate=true;MultipleActiveResultSets=true");
                });

                // Configurar logging mínimo para tests
                services.AddLogging(builder => builder.AddConsole().SetMinimumLevel(LogLevel.Warning));
            });

            // Configuración para tests
            builder.UseEnvironment("Testing");
        });

        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task Get_DatabaseConnection_Should_Return_Success()
    {
        // Act
        var response = await _client.GetAsync("/api/test/database-connection");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("Conexión a base de datos exitosa");
        content.Should().Contain("DatabaseExists");
        content.Should().Contain("DatabaseCreated");
        content.Should().Contain("Timestamp");
    }

    [Fact]
    public async Task Get_DatabaseConnection_Should_Create_New_Database()
    {
        // Act
        var response = await _client.GetAsync("/api/test/database-connection");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadAsStringAsync();
        var jsonDocument = JsonDocument.Parse(content);

        // La base de datos debería ser creada la primera vez (DatabaseCreated = true)
        var databaseCreated = jsonDocument.RootElement.GetProperty("databaseCreated").GetBoolean();
        databaseCreated.Should().BeTrue("La base de datos de prueba debería ser creada la primera vez");

        // DatabaseExists debería ser false (porque acabamos de crearla)
        var databaseExists = jsonDocument.RootElement.GetProperty("databaseExists").GetBoolean();
        databaseExists.Should().BeFalse("La base de datos no debería existir antes de crearla");
    }

    [Fact]
    public async Task Get_DatabaseConnection_Should_Not_Create_Database_On_Second_Call()
    {
        // Arrange - Primera llamada para crear la base de datos
        await _client.GetAsync("/api/test/database-connection");

        // Act - Segunda llamada
        var response = await _client.GetAsync("/api/test/database-connection");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadAsStringAsync();
        var jsonDocument = JsonDocument.Parse(content);

        // La segunda vez, la base de datos ya existe (DatabaseCreated = false)
        var databaseCreated = jsonDocument.RootElement.GetProperty("databaseCreated").GetBoolean();
        databaseCreated.Should().BeFalse("La base de datos no debería ser creada en la segunda llamada");

        // DatabaseExists debería ser true (porque ya existe)
        var databaseExists = jsonDocument.RootElement.GetProperty("databaseExists").GetBoolean();
        databaseExists.Should().BeTrue("La base de datos debería existir en la segunda llamada");
    }

    [Fact]
    public async Task Get_SerilogTest_Should_Return_Success()
    {
        // Act
        var response = await _client.GetAsync("/api/test/serilog-test");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("Serilog configurado correctamente");
    }

    [Fact]
    public async Task Get_AutoMapperTest_Should_Return_Success()
    {
        // Act
        var response = await _client.GetAsync("/api/test/automapper-test");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("AutoMapper configurado correctamente");
    }

    [Fact]
    public async Task Post_FluentValidationTest_With_Valid_Data_Should_Return_Success()
    {
        // Arrange
        var validTestObject = new
        {
            id = 1,
            name = "Café de Prueba",
            price = 4.50m,
            createdAt = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ")
        };

        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        var json = JsonSerializer.Serialize(validTestObject, options);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync("/api/test/fluentvalidation-test", content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var responseContent = await response.Content.ReadAsStringAsync();
        responseContent.Should().Contain("FluentValidation configurado correctamente");

        var jsonDocument = JsonDocument.Parse(responseContent);
        var isValid = jsonDocument.RootElement.GetProperty("isValid").GetBoolean();
        isValid.Should().BeTrue("Los datos válidos deberían pasar la validación");
    }

    [Fact]
    public async Task Post_FluentValidationTest_With_Invalid_Data_Should_Return_BadRequest()
    {
        // Arrange
        var invalidTestObject = new
        {
            id = 0, // ❌ Inválido
            name = "", // ❌ Vacío
            price = -10.00m, // ❌ Negativo
            createdAt = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ")
        };

        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        var json = JsonSerializer.Serialize(invalidTestObject, options);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync("/api/test/fluentvalidation-test", content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var responseContent = await response.Content.ReadAsStringAsync();
        responseContent.Should().Contain("Validación falló");

        var jsonDocument = JsonDocument.Parse(responseContent);
        var isValid = jsonDocument.RootElement.GetProperty("isValid").GetBoolean();
        isValid.Should().BeFalse("Los datos inválidos no deberían pasar la validación");

        // Verificar que hay errores específicos
        var errors = jsonDocument.RootElement.GetProperty("errors");
        errors.GetArrayLength().Should().BeGreaterThan(0, "Debería haber errores de validación");
    }

    [Theory]
    [InlineData("/api/test/database-connection")]
    [InlineData("/api/test/serilog-test")]
    [InlineData("/api/test/automapper-test")]
    public async Task Get_Endpoints_Should_Return_Success(string endpoint)
    {
        // Act
        var response = await _client.GetAsync(endpoint);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task API_Should_Return_Json_Content_Type()
    {
        // Act
        var response = await _client.GetAsync("/api/test/serilog-test");

        // Assert
        response.Content.Headers.ContentType?.MediaType.Should().Be("application/json");
    }

    [Fact]
    public async Task Database_Connection_Response_Should_Have_Expected_Properties()
    {
        // Act
        var response = await _client.GetAsync("/api/test/database-connection");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadAsStringAsync();

        using var jsonDocument = JsonDocument.Parse(content);
        var root = jsonDocument.RootElement;

        // Verificar propiedades específicas de tu implementación
        root.TryGetProperty("message", out _).Should().BeTrue("Debería tener propiedad 'message'");
        root.TryGetProperty("databaseExists", out _).Should().BeTrue("Debería tener propiedad 'databaseExists'");
        root.TryGetProperty("databaseCreated", out _).Should().BeTrue("Debería tener propiedad 'databaseCreated'");
        root.TryGetProperty("timestamp", out _).Should().BeTrue("Debería tener propiedad 'timestamp'");
    }

    public void Dispose()
    {
        // Limpiar la base de datos de prueba al terminar
        try
        {
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<CoffeeExpressDbContext>();
            context.Database.EnsureDeleted();
        }
        catch (Exception ex)
        {
            // Log error but don't fail the test
            Console.WriteLine($"Error cleaning up test database: {ex.Message}");
        }

        _client?.Dispose();
        _factory?.Dispose();
    }
}