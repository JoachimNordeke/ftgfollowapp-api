using System;
using API.Models.Users;

namespace API.Helpers
{
    public static class ExtensionMethods
    {
        public static User CreatePasswordHashAndSalt(this User user, string password)
        {
            if (string.IsNullOrWhiteSpace(password)) throw new ArgumentException("Value cannot be empty or whitespace only string.", "password");
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                user.PasswordSalt = hmac.Key;
                user.PasswordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }

            return user;
        }
    }
}
