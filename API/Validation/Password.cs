using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace API.Validation
{
    public class Password : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext context)
        {
            string password = (string)value;

            if (password.Length < 8) return new ValidationResult("The password must be at least 8 characters");
            else if (!password.Any(char.IsLetter)) return new ValidationResult("The password must contain at least one (1) letter");
            else if (!password.Any(char.IsDigit)) return new ValidationResult("The password must contain at least one (1) digit");
            else if (!password.Any(char.IsUpper)) return new ValidationResult("The password must contain at least one (1) upper case letter");
            else if (!password.Any(char.IsLower)) return new ValidationResult("The password must contain at least one (1) lower case letter");
            else if (password.Any(char.IsWhiteSpace)) return new ValidationResult("The password cannot contain any white spaces");
            else return null;
        }
    }
}
