
namespace CoffeeExpressAPI.Application.Dtos.User
{
    /// <summary>
    /// Dto de creación de usuario puede llegar a ser modificado a futuro
    /// </summary>
    public class CreateUserDto
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }
}
