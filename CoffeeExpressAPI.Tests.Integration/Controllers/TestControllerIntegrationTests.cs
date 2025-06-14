using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using FluentAssertions;
using System.Net;
using System.Text;
using System.Text.Json;
using CoffeeExpressAPI.Infrastructure.Data.Contexts;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Hosting;

namespace CoffeeExpressAPI.Tests.Integration.Controllers;

public class TestControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public TestControllerIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.UseEnvironment("Testing");
        });

        _client = _factory.CreateClient();

        // ✅ Llamar al método privado de esta clase
        InitializeDatabase();
    }

    // ✅ Este método SÍ existe (método privado)
    private void InitializeDatabase()
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<CoffeeExpressDbContext>();
        context.Database.EnsureCreated();
    }

    [Fact]
    public async Task Get_DatabaseConnection_Should_Return_Success()
    {
        // Act
        var response = await _client.GetAsync("/api/test/database-connection");

        // Debug: Ver el contenido de la respuesta si falla
        var content = await response.Content.ReadAsStringAsync();
        Console.WriteLine($"Response Status: {response.StatusCode}");
        Console.WriteLine($"Response Content: {content}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        content.Should().Contain("Conexión a base de datos exitosa");
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
            price = 4.50,
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
    }

    [Fact]
    public async Task Post_FluentValidationTest_With_Invalid_Data_Should_Return_BadRequest()
    {
        // Arrange
        var invalidTestObject = new
        {
            id = 0, // ❌ Inválido
            name = "", // ❌ Vacío
            price = -10.00, // ❌ Negativo
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
    }

    [Theory]
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
    public async Task Database_Should_Be_InMemory_During_Tests()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<CoffeeExpressDbContext>();

        // Act
        var databaseProvider = context.Database.ProviderName;

        // Assert
        databaseProvider.Should().Be("Microsoft.EntityFrameworkCore.InMemory");
    }

    [Fact]
    public async Task Database_Should_Be_Empty_Initially()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<CoffeeExpressDbContext>();

        // Act - Como no tenemos entidades todavía, solo verificamos que el contexto funciona
        var canConnect = await context.Database.CanConnectAsync();

        // Assert
        canConnect.Should().BeTrue();
    }
}