using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using CoffeeExpressAPI.Application.Mappings;

namespace CoffeeExpressAPI.Tests.Unit.Common;

/// <summary>
/// Clase base para tests unitarios que proporciona configuraciones comunes
/// </summary>
public abstract class BaseUnitTest : IDisposable
{
    protected readonly IMapper Mapper;
    protected readonly IServiceProvider ServiceProvider;

    protected BaseUnitTest()
    {
        var services = new ServiceCollection();

        // Configurar AutoMapper
        services.AddAutoMapper(typeof(ApplicationMappingProfile));

        ServiceProvider = services.BuildServiceProvider();
        Mapper = ServiceProvider.GetRequiredService<IMapper>();
    }

    /// <summary>
    /// Crea un objeto de prueba válido para TestSource
    /// </summary>
    protected static ApplicationMappingProfile.TestSource CreateValidTestSource(
        int id = 1,
        string name = "Café de Prueba",
        decimal price = 4.50m,
        DateTime? createdAt = null)
    {
        return new ApplicationMappingProfile.TestSource
        {
            Id = id,
            Name = name,
            Price = price,
            CreatedAt = createdAt ?? DateTime.UtcNow
        };
    }

    /// <summary>
    /// Crea un objeto de prueba inválido para TestSource
    /// </summary>
    protected static ApplicationMappingProfile.TestSource CreateInvalidTestSource()
    {
        return new ApplicationMappingProfile.TestSource
        {
            Id = 0, // ❌ Inválido
            Name = "", // ❌ Vacío
            Price = -10.00m, // ❌ Negativo
            CreatedAt = DateTime.UtcNow
        };
    }

    /// <summary>
    /// Libera recursos al finalizar el test
    /// </summary>
    public void Dispose()
    {
        if (ServiceProvider is IDisposable disposable)
        {
            disposable.Dispose();
        }
        GC.SuppressFinalize(this);
    }
}