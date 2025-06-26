using CoffeeExpressAPI.Application.Interfaces.Repositories;
using CoffeeExpressAPI.Domain.Entities;
using CoffeeExpressAPI.Infrastructure.Data.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CoffeeExpressAPI.Infrastructure.Repositories
{
    /// <summary>
    /// Implementación base abstracta del patrón Repository para Entity Framework.
    /// Proporciona operaciones CRUD comunes con soft delete, logging y manejo automático de fechas.
    /// </summary>
    /// <typeparam name="T">Entidad del dominio que hereda de BaseEntity</typeparam>
    public abstract class BaseRepository<T> : IRepository<T> where T : BaseEntity
    {
        protected readonly CoffeeExpressDbContext _context;
        protected readonly DbSet<T> _dbSet;
        protected readonly ILogger<BaseRepository<T>> _logger;

        /// <summary>
        /// Constructor base que inicializa el contexto, DbSet y logger
        /// </summary>
        protected BaseRepository(CoffeeExpressDbContext context, ILogger<BaseRepository<T>> logger)
        {
            _context = context;
            _dbSet = context.Set<T>();
            _logger = logger;
        }

        /// <summary>
        /// Obtiene una entidad por ID excluyendo registros marcados como eliminados
        /// </summary>
        public virtual async Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            try
            {
                return await _dbSet
                    .Where(e => !e.IsDeleted)
                    .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting entity {EntityType} by id {Id}", typeof(T).Name, id);
                throw;
            }
        }

        /// <summary>
        /// Obtiene todas las entidades excluyendo registros marcados como eliminados
        /// </summary>
        public virtual async Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Where(e => !e.IsDeleted)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Agrega una nueva entidad estableciendo CreatedAt y IsDeleted automáticamente
        /// </summary>
        public virtual async Task<T> AddAsync(T entity, CancellationToken cancellationToken = default)
        {
            entity.CreatedAt = DateTime.UtcNow;
            entity.IsDeleted = false;

            await _dbSet.AddAsync(entity, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Created new {EntityType} with id {Id}", typeof(T).Name, entity.Id);
            return entity;
        }

        /// <summary>
        /// Actualiza una entidad estableciendo UpdatedAt automáticamente
        /// </summary>
        public virtual async Task<T> UpdateAsync(T entity, CancellationToken cancellationToken = default)
        {
            entity.UpdatedAt = DateTime.UtcNow;

            _dbSet.Update(entity);
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Updated {EntityType} with id {Id}", typeof(T).Name, entity.Id);
            return entity;
        }

        /// <summary>
        /// Realiza eliminación lógica (soft delete) marcando IsDeleted como true
        /// </summary>
        public virtual async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            var entity = await GetByIdAsync(id, cancellationToken);
            if (entity != null)
            {
                // Soft delete
                entity.IsDeleted = true;
                entity.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Soft deleted {EntityType} with id {Id}", typeof(T).Name, id);
            }
        }

        /// <summary>
        /// Verifica si existe una entidad por ID excluyendo registros eliminados
        /// </summary>
        public virtual async Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Where(e => !e.IsDeleted)
                .AnyAsync(e => e.Id == id, cancellationToken);
        }
    }
}
