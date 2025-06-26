using CoffeeExpressAPI.Application.Dtos.User;
using CoffeeExpressAPI.Domain.Entities;

namespace CoffeeExpressAPI.Application.Interfaces.Services
{
    /// <summary>
    /// Servicio específico para el manejo de usuarios en CoffeeExpress.
    /// Hereda operaciones CRUD básicas y extiende con funcionalidades específicas del dominio de usuarios.
    /// </summary>
    public interface IUserService : IService<User, UserDto, CreateUserDto, UpdateUserDto>
    {
        /// <summary>Busca un usuario por su dirección de correo electrónico</summary>
        /// <param name="email">Dirección de correo electrónico del usuario</param>
        /// <param name="cancellationToken">Token de cancelación para operaciones asíncronas</param>
        /// <returns>DTO del usuario encontrado o null si no existe</returns>
        Task<UserDto?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);

        /// <summary>Obtiene todos los usuarios que se encuentran en estado activo</summary>
        /// <param name="cancellationToken">Token de cancelación para operaciones asíncronas</param>
        /// <returns>Colección de DTOs de usuarios activos</returns>
        Task<IEnumerable<UserDto>> GetActiveUsersAsync(CancellationToken cancellationToken = default);

        /// <summary>Verifica si una dirección de correo electrónico está disponible para registro</summary>
        /// <param name="email">Dirección de correo electrónico a verificar</param>
        /// <param name="cancellationToken">Token de cancelación para operaciones asíncronas</param>
        /// <returns>True si el email está disponible, False si ya está en uso</returns>
        Task<bool> IsEmailAvailableAsync(string email, CancellationToken cancellationToken = default);

        /// <summary>Desactiva un usuario cambiando su estado a inactivo sin eliminarlo</summary>
        /// <param name="id">Identificador único del usuario a desactivar</param>
        /// <param name="cancellationToken">Token de cancelación para operaciones asíncronas</param>
        Task DeactivateUserAsync(int id, CancellationToken cancellationToken = default);
    }
}
