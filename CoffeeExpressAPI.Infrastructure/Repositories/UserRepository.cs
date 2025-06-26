using CoffeeExpressAPI.Application.Interfaces.Repositories;
using CoffeeExpressAPI.Domain.Entities;
using CoffeeExpressAPI.Infrastructure.Data.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CoffeeExpressAPI.Infrastructure.Repositories
{
    /// <summary>
    /// Repositorio específico para usuarios con consultas especializadas del dominio.
    /// Hereda operaciones CRUD básicas de BaseRepository y agrega funcionalidades específicas de usuarios.
    /// </summary>
    public class UserRepository : BaseRepository<User>, IUserRepository
    {
        public UserRepository(CoffeeExpressDbContext context, ILogger<UserRepository> logger) : base(context, logger)
        {
        }

        /// <summary>
        /// Busca un usuario por email con comparación insensible a mayúsculas/minúsculas
        /// </summary>
        public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Where(u => !u.IsDeleted)
                .FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower(), cancellationToken);
        }

        /// <summary>
        /// Obtiene usuarios activos ordenados por nombre, excluyendo eliminados e inactivos
        /// </summary>
        public async Task<IEnumerable<User>> GetActiveUsersAsync(CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Where(u => !u.IsDeleted && u.IsActive)
                .OrderBy(u => u.FirstName)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Verifica si un email ya está registrado con comparación insensible a mayúsculas/minúsculas
        /// </summary>
        public async Task<bool> IsEmailTakenAsync(string email, CancellationToken cancellationToken = default)
        {
            return await _dbSet
           .Where(u => !u.IsDeleted)
           .AnyAsync(u => u.Email.ToLower() == email.ToLower(), cancellationToken);
        }

        /// <summary>
        /// Obtiene usuarios registrados en un rango de fechas ordenados por fecha de creación
        /// </summary>
        public async Task<IEnumerable<User>> GetUserByDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Where(u => !u.IsDeleted && u.CreatedAt >= startDate && u.CreatedAt <= endDate)
                .OrderBy(u => u.CreatedAt)
                .ToListAsync(cancellationToken);
        }
    }
}