using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace CoffeeExpressAPI.Application.Mappings
{
    /// <summary>
    /// Clase de configuración para AutoMapper.
    /// Proporciona métodos de extensión para registrar AutoMapper en el contenedor de DI.
    /// </summary>
    public static class AutoMapperConfig
    {
        /// <summary>
        /// Registra AutoMapper en el contenedor de servicios.
        /// </summary>
        /// <param name="services">Colección de servicios</param>
        /// <returns>La colección de servicios para encadenamiento</returns>
        public static IServiceCollection AddAutoMapperConfiguration(this IServiceCollection services)
        {
            // Configurar AutoMapper para buscar perfiles en el Assembly de Application
            services.AddAutoMapper(typeof(ApplicationMappingProfile).Assembly);
            return services;
        }

        /// <summary>
        /// Obtiene todos los perfiles de mapeo del Assembly de Application.
        /// </summary>
        /// <returns>Lista de tipos que heredan de Profile</returns>
        public static IEnumerable<Type> GetMappingProfiles()
        {
            return Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(type => typeof(Profile).IsAssignableFrom(type) && !type.IsAbstract);
        }
    }
}
