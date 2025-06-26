using AutoMapper;
using CoffeeExpressAPI.Application.Dtos.User;
using CoffeeExpressAPI.Application.Interfaces.Repositories;
using CoffeeExpressAPI.Domain.Entities;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoffeeExpressAPI.Infrastructure.Services
{
    /// <summary>
    /// Servicio específico para el manejo de usuarios con lógica de negocio especializada.
    /// Hereda operaciones CRUD básicas y agrega funcionalidades como validación de email y desactivación de usuarios.
    /// </summary>
    public class UserService : BaseService<User, UserDto, CreateUserDto, UpdateUserDto>
    {
        private readonly IUserRepository _userRepository;

        public UserService(
            IUserRepository userRepository,
            IMapper mapper,
            ILogger<UserService> logger) : base(userRepository, mapper, logger)
        {
            _userRepository = userRepository;
        }

        /// <summary>
        /// Busca un usuario por email y lo convierte a DTO, retorna null si no existe
        /// </summary>
        public async Task<UserDto?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
        {
            var user = await _userRepository.GetByEmailAsync(email, cancellationToken);
            return user != null ? _mapper.Map<UserDto>(user) : null;
        }

        /// <summary>
        /// Obtiene solo usuarios activos y los convierte a DTOs
        /// </summary>
        public async Task<IEnumerable<UserDto>> GetActiveUsersAsync(CancellationToken cancellationToken = default)
        {
            var users = await _userRepository.GetActiveUsersAsync(cancellationToken);
            return _mapper.Map<IEnumerable<UserDto>>(users);
        }

        /// <summary>
        /// Verifica si un email está disponible para registro (inverso de IsEmailTaken)
        /// </summary>
        public async Task<bool> IsEmailAvailableAsync(string email, CancellationToken cancellationToken = default)
        {
            return !await _userRepository.IsEmailTakenAsync(email, cancellationToken);
        }

        /// <summary>
        /// Desactiva un usuario cambiando IsActive a false sin eliminarlo físicamente
        /// </summary>
        public async Task DeactivateUserAsync(int id, CancellationToken cancellationToken = default)
        {
            var user = await _userRepository.GetByIdAsync(id, cancellationToken);
            if (user == null)
                throw new InvalidOperationException($"User with ID {id} not found");
            user.IsActive = false;
            await _userRepository.UpdateAsync(user, cancellationToken);
            _logger.LogInformation("Deactivated user with ID: {UserId}", id);
        }

        /// <summary>
        /// Validación personalizada para creación de usuarios: verifica que el email no esté en uso
        /// </summary>
        protected override async Task ValidateForCreationAsync(CreateUserDto createDto, CancellationToken cancellationToken)
        {
            if (await _userRepository.IsEmailTakenAsync(createDto.Email, cancellationToken))
                throw new InvalidOperationException($"Email {createDto.Email} is already taken");
        }
    }
}