using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using API.Models.Users;
using DocumentDb;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthenticationController : BaseController
    {
        private readonly IReadRepository<User> _readRepository;

        public AuthenticationController()
        {
            _readRepository = DocumentDatabase.GetReadRepository<User>();
        }

        [HttpPost]
        public async Task<IActionResult> Token([FromForm]string grant_type, [FromForm]string email, [FromForm]string password, [FromForm]string refresh_token)
        {
            object userResponse = null;

            if (grant_type == "refresh_token")
            {
                var userId = GetUserIdFromRefreshToken(refresh_token);

                if (userId == null)
                {
                    return BadRequest();
                }

                var user = await _readRepository.GetAsync(userId);

                userResponse = new { IdToken = GetIdToken(user), RefreshToken = GetRefreshToken(user) };
            }

            if (grant_type == "token")
            {
                var user = (await _readRepository.FindAsync(x => x.Email == email)).SingleOrDefault();

                if (user == null)
                {
                    return BadRequest();
                }

                var passwordHash = GetPasswordHash(password, user.PasswordSalt);

                for (int i = 0; i < passwordHash.Length; i++)
                {
                    if (passwordHash[i] != user.PasswordHash[i])
                    {
                        return BadRequest();
                    }
                }

                userResponse = new { IdToken = GetIdToken(user), RefreshToken = GetRefreshToken(user) };

            }

            return Ok(userResponse);
        }

        private string GetIdToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var secret = Environment.GetEnvironmentVariable("TOKEN_SECRET");
            var key = Encoding.ASCII.GetBytes(secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.Id),
                    new Claim(ClaimTypes.Role, user.Role),
                    new Claim("firstname", user.Firstname),
                    new Claim("storeId", user.MainStoreId)
                }),
                Expires = DateTime.Now.AddHours(2),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        private string GetRefreshToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var secret = Environment.GetEnvironmentVariable("TOKEN_SECRET");
            var key = Encoding.ASCII.GetBytes(secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.Id)
                }),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        private string GetUserIdFromRefreshToken(string refreshToken)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var secret = Environment.GetEnvironmentVariable("TOKEN_SECRET");
            var key = Encoding.ASCII.GetBytes(secret);
            var validations = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false
            };
            var claims = tokenHandler.ValidateToken(refreshToken, validations, out var token);

            if (!claims.Identity.IsAuthenticated)
            {
                return null;
            }

            return claims.Identity.Name;
        }

        private byte[] GetPasswordHash(string password, byte[] salt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512(salt))
            {
                return hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            }
        }
    }
}