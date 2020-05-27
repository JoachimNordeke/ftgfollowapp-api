using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace API.Validation
{
    public class Username : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext context)
        {
            string username = (string)value;

            if (username.Length < 6) return new ValidationResult("The username must be at least 6 characters");
            else if (username.Any(x => !char.IsLetterOrDigit(x))) return new ValidationResult("The username can only consist of letters and digits.");
            else if (username.Any(char.IsWhiteSpace)) return new ValidationResult("The username cannot contain any white spaces");
            else return null;
        }
    }
}
