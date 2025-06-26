using CoffeeExpressAPI.Domain.Entities;

namespace CoffeeExpressAPI.Application.Interfaces.Repositories
{
    /// <summary>
    /// Interfaz especifica para el manejo de entidades User.
    /// Extiende la funcionalidad con métodos específicos del dominio de usuarios.
    /// </summary>
    public interface IUserRepository : IRepository<User>
    {
        //Métodos específicos del dominio de Usuario

        /// <summary>
        /// Busca un usuario por su dirección de correo electrónico.
        /// </summary>
        /// <param name="email">Dirección de correo electrónico del usuario</param>
        /// <param name="cancellationToken">Token de cancelación para operaciones asíncronas</param>
        /// <returns>El usuario encontrado o null si no existe</returns>
        Task<User> GetByEmailAsync(string email, CancellationToken cancellationToken = default);

        /// <summary>
        /// Obtiene todos los usuarios que se encuentran en estado activo.
        /// </summary>
        /// <param name="cancellationToken">Token de cancelación para operaciones asíncronas</param>
        /// <returns>Colección de usuarios activos</returns>
        Task<IEnumerable<User>> GetActiveUsersAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Verifica si una dirección de correo electrónico ya está registrada en el sistema.
        /// </summary>
        /// <param name="email">Dirección de correo electrónico a verificar</param>
        /// <param name="cancellationToken">Token de cancelación para operaciones asíncronas</param>
        /// <returns>True si el email ya está en uso, False si está disponible</returns>
        Task<bool> IsEmailTakenAsync(string email, CancellationToken cancellationToken = default);

        /// <summary>
        /// Obtiene usuarios registrados dentro de un rango de fechas específico.
        /// </summary>
        /// <param name="startDate">Fecha inicial del rango (inclusiva)</param>
        /// <param name="endDate">Fecha final del rango (inclusiva)</param>
        /// <param name="cancellationToken">Token de cancelación para operaciones asíncronas</param>
        /// <returns>Colección de usuarios registrados en el rango de fechas especificado</returns>
        Task<IEnumerable<User>> GetUserByDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
    }
}
