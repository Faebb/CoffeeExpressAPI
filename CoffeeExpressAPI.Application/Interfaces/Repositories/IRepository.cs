using CoffeeExpressAPI.Domain.Entities;

namespace CoffeeExpressAPI.Application.Interfaces.Repositories
{
    /// <summary>
    /// Interfaz base genérica.
    /// Proporciona las operaciones CRUD estándar para cualquier entidad del dominio.
    /// </summary>
    /// <typeparam name="T">Tipo de entidad que debe heredar de BaseEntity. </typeparam>
    public interface IRepository<T> where T : BaseEntity
    {
        // Operacioens básicas CRUD

        /// <summary>
        /// Obtiene una entidad por su identificador único
        /// </summary>
        /// <param name="id">Identificador de la entidad</param>
        /// <param name="cancellationToken">Token de cancelación para operaciones asíncronas</param>
        /// <returns>La entidad encontrada o null si no existe</returns>
        Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

        /// <summary>
        /// Obtiene todas las entidades del tipo especificado
        /// </summary>
        /// <param name="cancellationToken">Token de cancelación para operaciones asíncronas</param>
        /// <returns>Colección de todas las entidades</returns>
        Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Agrega una nueva entidad al repositorio
        /// </summary>
        /// <param name="entity">Entidad a agregar</param>
        /// <param name="cancellationToken">Token de cancelación para operaciones asíncronas</param>
        /// <returns>La entidad agregada con su Id asignado</returns>
        Task<T> AddAsync(T entity, CancellationToken cancellationToken = default);

        /// <summary>
        /// Actualiza una entidad existente
        /// </summary>
        /// <param name="entity">Entidad con los datos actualizados</param>
        /// <param name="cancellationToken">Token de cancelación para operaciones asíncronas</param>
        /// <returns>La entidad actualizada</returns>
        Task<T> UpdateAsync(T entity, CancellationToken cancellationToken = default);

        /// <summary>
        /// Elimina una entidad por su identificador
        /// </summary>
        /// <param name="id">Identificador de la entidad a eliminar</param>
        /// <param name="cancellationToken">Token de cancelación para operaciones asíncronas</param>
        Task DeleteAsync(int id, CancellationToken cancellationToken = default);

        /// <summary>
        /// Verifica si existe una entidad con el identificador especificado
        /// </summary>
        /// <param name="id">Identificador de la entidad</param>
        /// <param name="cancellationToken">Token de cancelación para operaciones asíncronas</param>
        /// <returns>True si la entidad existe, False en caso contrario</returns>
        Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default);
    }
}
