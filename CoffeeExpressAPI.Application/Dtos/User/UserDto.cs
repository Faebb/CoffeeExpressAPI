
namespace CoffeeExpressAPI.Application.Dtos.User
{
    /// <summary>
    /// Dto general de usuario puede incurrir en futuras actualizaciones
    /// </summary>
    public class UserDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public string FullName => $"{FirstName}{LastName}";
    }
}
