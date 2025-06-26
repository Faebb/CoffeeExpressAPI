using AutoMapper;
using CoffeeExpressAPI.Application.Interfaces.Repositories;
using CoffeeExpressAPI.Application.Interfaces.Services;
using CoffeeExpressAPI.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace CoffeeExpressAPI.Infrastructure.Services
{
    /// <summary>
    /// Implementación base abstracta de servicios de aplicación con operaciones CRUD.
    /// Maneja la lógica de negocio común, transformación de entidades a DTOs y validaciones extensibles.
    /// </summary>
    /// <typeparam name="TEntity">Entidad del dominio</typeparam>
    /// <typeparam name="TDto">DTO para operaciones de lectura</typeparam>
    /// <typeparam name="TCreateDto">DTO para operaciones de creación</typeparam>
    /// <typeparam name="TUpdateDto">DTO para operaciones de actualización</typeparam>
    public abstract class BaseService<TEntity, TDto, TCreateDto, TUpdateDto> : IService<TEntity, TDto, TCreateDto, TUpdateDto>
        where TEntity : BaseEntity
        where TDto : class
        where TCreateDto : class
        where TUpdateDto : class
    {
        protected readonly IRepository<TEntity> _repository;
        protected readonly IMapper _mapper;
        protected readonly ILogger<BaseService<TEntity, TDto, TCreateDto, TUpdateDto>> _logger;

        /// <summary>
        /// Constructor base que inicializa las dependencias del servicio
        /// </summary>
        protected BaseService(
            IRepository<TEntity> repository,
            IMapper mapper,
            ILogger<BaseService<TEntity, TDto, TCreateDto, TUpdateDto>> logger)
        {
            _repository = repository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene una entidad por ID y la convierte a DTO, lanza excepción si no existe
        /// </summary>
        public virtual async Task<TDto> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var entity = await _repository.GetByIdAsync(id, cancellationToken);
            if (entity == null)
                throw new InvalidOperationException($"{typeof(TEntity).Name} with ID {id} not found");
            return _mapper.Map<TDto>(entity);
        }

        /// <summary>
        /// Obtiene todas las entidades y las convierte a DTOs
        /// </summary>
        public virtual async Task<IEnumerable<TDto>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var entities = await _repository.GetAllAsync(cancellationToken);
            return _mapper.Map<IEnumerable<TDto>>(entities);
        }

        /// <summary>
        /// Crea una nueva entidad con validaciones previas y logging automático
        /// </summary>
        public virtual async Task<TDto> CreateAsync(TCreateDto createDto, CancellationToken cancellationToken = default)
        {
            // Validaciones previas a la creación (override en clases derivadas)
            await ValidateForCreationAsync(createDto, cancellationToken);
            var entity = _mapper.Map<TEntity>(createDto);
            var createdEntity = await _repository.AddAsync(entity, cancellationToken);
            _logger.LogInformation("Created {EntityType} with ID: {Id}", typeof(TEntity).Name, createdEntity.Id);
            return _mapper.Map<TDto>(createdEntity);
        }

        /// <summary>
        /// Actualiza una entidad existente con validaciones y verificación de existencia
        /// </summary>
        public virtual async Task<TDto> UpdateAsync(int id, TUpdateDto updateDto, CancellationToken cancellationToken = default)
        {
            var existingEntity = await _repository.GetByIdAsync(id, cancellationToken);
            if (existingEntity == null)
                throw new InvalidOperationException($"{typeof(TEntity).Name} with ID {id} not found");
            // Validaciones previas a la actualización
            await ValidateForUpdateAsync(id, updateDto, cancellationToken);
            _mapper.Map(updateDto, existingEntity);
            var updatedEntity = await _repository.UpdateAsync(existingEntity, cancellationToken);
            return _mapper.Map<TDto>(updatedEntity);
        }

        /// <summary>
        /// Elimina una entidad verificando su existencia previamente
        /// </summary>
        public virtual async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            if (!await _repository.ExistsAsync(id, cancellationToken))
                throw new InvalidOperationException($"{typeof(TEntity).Name} with ID {id} not found");
            await _repository.DeleteAsync(id, cancellationToken);
            _logger.LogInformation("Deleted {EntityType} with ID: {Id}", typeof(TEntity).Name, id);
        }

        /// <summary>
        /// Método virtual para validaciones personalizadas antes de crear entidades.
        /// Puede ser sobrescrito en servicios específicos para agregar lógica de negocio.
        /// </summary>
        protected virtual Task ValidateForCreationAsync(TCreateDto createDto, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// Método virtual para validaciones personalizadas antes de actualizar entidades.
        /// Puede ser sobrescrito en servicios específicos para agregar lógica de negocio.
        /// </summary>
        protected virtual Task ValidateForUpdateAsync(int id, TUpdateDto updateDto, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}