using CoffeeExpressAPI.Domain.Entities;

namespace CoffeeExpressAPI.Application.Interfaces.Services
{
    /// <summary>
    /// Interfaz base genérica para servicios de aplicación que implementa operaciones CRUD.
    /// Maneja la lógica fe negocio y tranformacion entre entidades y DTOs
    /// </summary>
    /// <typeparam name="TEntity">Entidad del dominio que hereda de BaseEntity</typeparam>
    /// <typeparam name="TDto">DTO para operaciones de lectura</typeparam>
    /// <typeparam name="TCreateDto">DTO para operaciones de creación</typeparam>
    /// <typeparam name="TUpdateDto">DTO para operaciones de actualización</typeparam>
    public interface IService<TEntity, TDto, TCreateDto, TUpdateDto> 
        where TEntity : BaseEntity
        where TDto : class
        where TCreateDto : class
        where TUpdateDto : class
    {
        /// <summary>
        /// Obtiene una entidad por ID y la retorna como DTO
        /// </summary>
        /// <param name="id">Identificador único de la entidad</param>
        /// <param name="cancellationToken">Token de cancelación para operaciones asíncronas</param>
        /// <returns>DTO de la entidad encontrada</returns>
        Task<TDto> GetByIdAsync(int id, CancellationToken cancellationToken = default);

        /// <summary>
        /// Obtiene todas las entidades como colección de DTOs
        /// </summary>
        /// <param name="cancellationToken">Token de cancelación para operaciones asíncronas</param>
        /// <returns>Colección de DTOs de todas las entidades</returns>
        Task<IEnumerable<TDto>> GetAllAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Crea una nueva entidad a partir de un DTO de creación
        /// </summary>
        /// <param name="createDto">DTO con los datos para crear la entidad</param>
        /// <param name="cancellationToken">Token de cancelación para operaciones asíncronas</param>
        /// <returns>DTO de la entidad creada</returns>
        Task<TDto> CreateAsync(TCreateDto createDto, CancellationToken cancellationToken = default);

        /// <summary>Actualiza una entidad existente con los datos del DTO de actualización</summary>
        /// <param name="id">Identificador único de la entidad a actualizar</param>
        /// <param name="updateDto">DTO con los datos actualizados</param>
        /// <param name="cancellationToken">Token de cancelación para operaciones asíncronas</param>
        /// <returns>DTO de la entidad actualizada</returns>
        Task<TDto> UpdateAsync(int id, TUpdateDto updateDto, CancellationToken cancellationToken = default);

        /// <summary>Elimina una entidad por su ID</summary>
        /// <param name="id">Identificador único de la entidad a eliminar</param>
        /// <param name="cancellationToken">Token de cancelación para operaciones asíncronas</param>
        Task DeleteAsync(int id, CancellationToken cancellationToken = default);
    }
}
