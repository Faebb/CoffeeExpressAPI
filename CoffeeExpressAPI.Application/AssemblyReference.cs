using System.Reflection;

namespace CoffeeExpressAPI.Application
{
    /// <summary>
    /// Clase de referencia para acceder al Assembly de Application.
    /// Utilizada principalmente para configurar MediatR y otros servicios
    /// que necesitan escanear este Assembly.
    /// </summary>
    public static class AssemblyReference
    {
        /// <summary>
        /// Referencia al Assembly actual de Application.
        /// </summary>
        public static readonly Assembly Assembly = typeof(Assembly).Assembly;
    }
}
