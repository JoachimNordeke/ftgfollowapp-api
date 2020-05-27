using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using API.Constants;
using API.Factories;
using API.Helpers;
using API.Models.Stores;
using API.Models.Users;
using DocumentDb;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : BaseController
    {
        private readonly IReadRepository<User> _readUserRepository;
        private readonly IWriteRepository<User> _writeUserRepository;
        private readonly IReadRepository<EmailValidation> _readEmailValidationRepository;
        private readonly IWriteRepository<EmailValidation> _writeEmailValidationRepository;
        private readonly IReadRepository<Store> _readStoreRepository;

        public UsersController()
        {
            _readUserRepository = DocumentDatabase.GetReadRepository<User>();
            _writeUserRepository = DocumentDatabase.GetWriteRepository<User>();
            _readEmailValidationRepository = DocumentDatabase.GetReadRepository<EmailValidation>();
            _writeEmailValidationRepository = DocumentDatabase.GetWriteRepository<EmailValidation>();
            _readStoreRepository = DocumentDatabase.GetReadRepository<Store>();
        }

        //[AllowAnonymous]
        //[HttpPost("reset-password")]
        //public async Task<IActionResult> ResetPassword([FromBody]ResetPassword reset)
        //{
        //    var user = (await _readRepository.FindAsync(x => x.Email == reset.Email)).SingleOrDefault();

        //    if (user == null)
        //    {
        //        return BadRequest();
        //    }

        //    var password = GeneratePassword();

        //    user.IsPasswordReset = true;
        //    user.CreatePasswordHashAndSalt(password);

        //    await _writeRepository.UpdateAsync(user);

        //await SendEmail(user.Email, user.Username, password);

        //    return Ok();
        //}

        //[HttpPut("change-password")]
        //public async Task<IActionResult> ChangePassword([FromBody]ChangePassword change)
        //{
        //    var user = await _readRepository.GetAsync(_authorizedUserId);

        //    var computedHash = GetPasswordHash(change.OldPassword, user.PasswordSalt);

        //    for (int i = 0; i < computedHash.Length; i++)
        //    {
        //        if (computedHash[i] != user.PasswordHash[i])
        //            return BadRequest(new { message = "Fel lösenord" }); ;
        //    }

        //    user.IsPasswordReset = false;
        //    user.CreatePasswordHashAndSalt(change.Password);

        //    await _writeRepository.UpdateAsync(user);

        //    return Ok();
        //}

        //[HttpPut("hide-message")]
        //public async Task HideMessage()
        //{
        //    var user = await _readRepository.GetAsync(_authorizedUserId);
        //    user.IsPasswordReset = false;
        //    await _writeRepository.UpdateAsync(user);
        //}

        [HttpPost]
        [Authorize(Roles = "storemanager,regionalmanager,admin")]
        public async Task<IActionResult> Create([FromBody]NewUpdateUser newUser)
        {
            var emailExists = (await _readUserRepository.FindAsync(x => x.Email == newUser.Email)).Count() > 0;

            if (emailExists)
            {
                return BadRequest(new { error = "e-mail exists" });
            }

            var user = UserFactory.CreateUser(newUser);

            try
            {
                await _writeUserRepository.CreateAsync(user);
            }
            catch
            {
                return BadRequest();
            }

            //var validation = await CreateEmailValidationCode(user);
            //await SendInvitationEmail(user, validation);

            return Ok();
        }

        [HttpPost("check-validation")]
        [AllowAnonymous]
        public async Task<IActionResult> CheckValidation([FromForm] string code, [FromForm] string userId)
        {
            await ValidationCodesCleanup();
            var validationExists = (await _readEmailValidationRepository.FindAsync(x => x.ValidationCode == code && x.UserId == userId)).Count() > 0;
            return Ok(validationExists);
        }

        [HttpPost("set-password")]
        [AllowAnonymous]
        public async Task<IActionResult> SetPassword(
            [FromForm] string code, 
            [FromForm] string userId, 
            [FromForm] string password, 
            [FromForm] string confirmPassword )
        {

            if(password.Length < 8 || password != confirmPassword)
            {
                return BadRequest();
            }

            var validation = (await _readEmailValidationRepository.FindAsync(x => x.ValidationCode == code && x.UserId == userId)).SingleOrDefault();
            if (validation == null)
            {
                return BadRequest();
            }

            var user = await _readUserRepository.GetAsync(userId);
            user.CreatePasswordHashAndSalt(password);
            user.IsActive = true;
            try
            {
                await _writeUserRepository.UpdateAsync(user);
            }
            catch
            {
                return BadRequest();
            }

            await _writeEmailValidationRepository.DeleteManyAsync(x => x.UserId == user.Id);

            return Ok();
        }

        private async Task<EmailValidation> CreateEmailValidationCode(User user)
        {
            var emailValidation = new EmailValidation
            {
                Id = Guid.NewGuid().ToString(),
                UserId = user.Id,
                ValidationCode = Guid.NewGuid().ToString()
            };
            await _writeEmailValidationRepository.CreateAsync(emailValidation);

            await ValidationCodesCleanup();

            return emailValidation;
        }

        private async Task ValidationCodesCleanup()
        {
            await _writeEmailValidationRepository.DeleteManyAsync(x => x.CreatedAtUtc < DateTime.UtcNow.AddDays(-7));
        }

        private async Task<Response> SendInvitationEmail(User user, EmailValidation validation)
        {
            var key = Environment.GetEnvironmentVariable("SENDGRID_API_KEY");
            var client = new SendGridClient(key);
            var from = new EmailAddress("no-reply@ftgfollowapp.com", "FTGFollowApp");
            var subject = "Nytt konto";
            var name = user.Firstname?.Trim() != null ? user.Firstname.Trim() : null;
            var to = new EmailAddress(user.Email, name != null ? name : user.Email);
            var textContent = $"Hej{(to.Name != null ? " " + to.Name : "")}! \n" +
                $"Gå in på denna adress för att aktivera ditt konto och skapa ett lösenord: http://localhost:3000/users/activate?code={validation.ValidationCode}&userId={user.Id}" +
                $"\n" +
                $"Mvh\n" +
                $"FTGFollowApp";
            var htmlContent = $"<h2>Hej{(to.Name != null ? " " + to.Name : "")}!</h2><br>" +
                $"<p>Gå in på denna adress för att aktivera ditt konto och skapa ett lösenord: " +
                $"<a href=\"http://localhost:3000/users/activate?code={validation.ValidationCode}&userId={user.Id}\">http://localhost:3000/users/activate?code={validation.ValidationCode}&userId={user.Id}</a></p><br><p>Mvh<br>FTGFollowApp</p>";
            var msg = MailHelper.CreateSingleEmail(from, to, subject, textContent, htmlContent);
            return await client.SendEmailAsync(msg);
        }

        //private UserDTO AssignToken(UserDTO user, string id, string role)
        //{
        //    var tokenHandler = new JwtSecurityTokenHandler();
        //    var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
        //    var tokenDescriptor = new SecurityTokenDescriptor
        //    {
        //        Subject = new ClaimsIdentity(new Claim[]
        //        {
        //            new Claim(ClaimTypes.Name, id)
        //        }),
        //        Expires = DateTime.Now.AddDays(14),
        //        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        //    };
        //    var token = tokenHandler.CreateToken(tokenDescriptor);
        //    user.Token = tokenHandler.WriteToken(token);

        //    return user;
        //}



        //private string GeneratePassword()
        //{
        //    const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789abcdefghijklmnopqrstuvwxyz0123456789";
        //    var rnd = new Random();

        //    return new string(Enumerable.Repeat(chars, 12)
        //    .Select(s => s[rnd.Next(s.Length)]).ToArray());
        //}

        //async Task<Response> SendEmail(string email, string username, string newPassword)
        //{
        //    var apiKey = _appSettings.SENDGRID_API_KEY;
        //    var client = new SendGridClient(apiKey);
        //    var from = new EmailAddress("no-reply@ftgfollowapp", "FTGFollowApp");
        //    var subject = "Lösenord återställt";
        //    var to = new EmailAddress(email, username);
        //    var plainTextContent = $"Hej {username}! \n" +
        //        $"Här är ditt nya lösenord: {newPassword}\n" +
        //        $"\n" +
        //        $"Mvh\n" +
        //        $"FTGFollowApp";
        //    var htmlContent = $"<h2>Hej {username}!</h2><br><p>Här är ditt nya lösenord: {newPassword}</p><br><p>Mvh<br>FTGFollowApp</p>";
        //    var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
        //    return await client.SendEmailAsync(msg);
        //}
    }
}
