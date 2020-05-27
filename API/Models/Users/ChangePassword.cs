using API.Validation;
using System.ComponentModel.DataAnnotations;

namespace API.Models.Users
{
    public class ChangePassword
    {
        [Required]
        public string OldPassword { get; set; }

        [Required, Password]
        public string Password { get; set; }

        [Required, Compare("Password")]
        public string ConfirmPassword { get; set; }
    }
}
