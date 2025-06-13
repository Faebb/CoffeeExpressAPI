using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;


namespace CoffeeExpressAPI.Application.Validators
{
    /// <summary>
    /// Configuración de FluentValidation para la aplicación
    /// </summary>
    public static class FluentValidationConfig
    {
        /// <summary>
        /// Registra FluentValidation en el contenedor de servicios
        /// </summary>
        /// <param name="services">Colección de servicios</param>
        /// <returns>La colección de servicios para encadenamiento</returns>
        public static IServiceCollection AddFluentValidationConfiguration(this IServiceCollection services)
        {
            // Registrar todos los validadores del Assembly de Application
            services.AddValidatorsFromAssembly(typeof(AssemblyReference).Assembly);

            // Configurar opciones globales de validación
            ValidatorOptions.Global.LanguageManager.Enabled = false; // Usamos mensajes personalizados
            ValidatorOptions.Global.DefaultRuleLevelCascadeMode = CascadeMode.Stop; // Parar en el primer error por regla
            ValidatorOptions.Global.DefaultClassLevelCascadeMode = CascadeMode.Continue; // Continuar validando otras propiedades

            return services;
        }

        /// <summary>
        /// Obtiene todos los validadores registrados en el Assembly
        /// </summary>
        /// <returns>Lista de tipos que implementan IValidator</returns>
        public static IEnumerable<Type> GetValidatorTypes()
        {
            return Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(type => type.GetInterfaces().Any(i =>
                    i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IValidator<>)))
                .Where(type => !type.IsAbstract && !type.IsInterface);
        }

        /// <summary>
        /// Valida un objeto usando su validador correspondiente
        /// </summary>
        /// <typeparam name="T">Tipo del objeto a validar</typeparam>
        /// <param name="serviceProvider">Proveedor de servicios</param>
        /// <param name="instance">Instancia a validar</param>
        /// <returns>Resultado de la validación</returns>
        public static async Task<FluentValidation.Results.ValidationResult> ValidateAsync<T>(
            IServiceProvider serviceProvider,
            T instance) where T : class
        {
            var validator = serviceProvider.GetService<IValidator<T>>();
            if (validator == null)
            {
                // Si no hay validador, se considera válido
                return new FluentValidation.Results.ValidationResult();
            }

            return await validator.ValidateAsync(instance);
        }
    }
}
