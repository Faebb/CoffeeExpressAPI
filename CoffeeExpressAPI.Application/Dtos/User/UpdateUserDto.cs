
namespace CoffeeExpressAPI.Application.Dtos.User
{
    /// <summary>
    /// Dto de actualización de usuario puede llegar a ser modificado a futuro
    /// </summary>
    public class UpdateUserDto
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
    }
}
