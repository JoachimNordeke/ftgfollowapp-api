using System.ComponentModel.DataAnnotations;

namespace API.Models.Users
{
    public class LoginUser
    {
        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
