using api.Models;
using api.Options;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Security.Permissions;
using System.Text;

namespace api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly TaskTrackerContext _context;
        private readonly IConfiguration _configuration;

        public AuthController(TaskTrackerContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpPost("signIn")]
        public async Task<ActionResult<User>> PostSignIn([FromBody]User userData)
        {
            if (userData != null && userData.Email != string.Empty && userData.Password != string.Empty)
            {
                User user = await _context.Users.FirstOrDefaultAsync(x => x.Email == userData.Email && x.IsConfrim == true);
                if (user != null)
                {
                    var jwtOptions = _configuration
                        .GetSection("JwtOptions")
                        .Get<JwtOptions>();

                    var claims = new[] {
                            new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                            new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
                            new Claim("user_id", user.Id.ToString()),
                        };

                    var keyBytes = Encoding.UTF8.GetBytes(jwtOptions.Key);
                    var symmetricKey = new SymmetricSecurityKey(keyBytes);

                    var signingCredentials = new SigningCredentials(symmetricKey, SecurityAlgorithms.HmacSha512);

                    var tokenDescriptor = new SecurityTokenDescriptor
                    {
                        Issuer = jwtOptions.Issuer,
                        Audience = jwtOptions.Audience,
                        Subject = new ClaimsIdentity(claims),
                        Expires = DateTime.UtcNow.AddSeconds(jwtOptions.ExpirationSeconds),
                        SigningCredentials = signingCredentials
                    };
                    var token = new JwtSecurityTokenHandler().CreateJwtSecurityToken(tokenDescriptor);
                    return Ok(new JwtSecurityTokenHandler().WriteToken(token));
                }

                return Conflict("Confrim your email");
            }

            return BadRequest();
        }

        [HttpPost("signUp")]
        public async Task<ActionResult<User>> PostSignUp([FromBody]User user)
        {
            if (user.Email != string.Empty && user.Password != string.Empty)
            {
                user.Salt = SecurityHelper.generateSalt(50);

                user.Password = SecurityHelper.hashPassword(user.Password, user.Salt);

                user.IsConfrim = false;

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                await Email.SendConfirmationEmail(user.Email, user.Id.ToString());

                return Ok("Confrim your email");
            }

            return BadRequest();
        }

        [HttpGet("confirmEmail")]
        public async Task<ActionResult> ConfirmEmail(string email, string userId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email && u.Id.ToString() == userId);
            if (user == null)
            {
                return BadRequest("Invalid email or user ID");
            }

            if ((bool)!user.IsConfrim)
            {
                user.IsConfrim = true;

                await _context.SaveChangesAsync();

                return Ok("Email confirmed successfully. You can now access your account.");
            }
            else
            {
                return BadRequest("Email has already been confirmed.");
            }
        }

        [HttpPost("refresh")]
        public async Task<ActionResult<Commit>> PostRefresh()
        {
            if (!Request.Headers.Authorization.IsNullOrEmpty())
            {
                var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == Options.Decoder.getUserId(Request.Headers.Authorization));

                if (user != null)
                {
                    var jwtOptions = _configuration
                       .GetSection("JwtOptions")
                       .Get<JwtOptions>();

                    var claims = new[] {
                            new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                            new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
                            new Claim("user_id", user.Id.ToString()),
                        };

                    var keyBytes = Encoding.UTF8.GetBytes(jwtOptions.Key);
                    var symmetricKey = new SymmetricSecurityKey(keyBytes);

                    var signingCredentials = new SigningCredentials(symmetricKey, SecurityAlgorithms.HmacSha512);

                    var tokenDescriptor = new SecurityTokenDescriptor
                    {
                        Issuer = jwtOptions.Issuer,
                        Audience = jwtOptions.Audience,
                        Subject = new ClaimsIdentity(claims),
                        Expires = DateTime.UtcNow.AddSeconds(jwtOptions.ExpirationSeconds),
                        SigningCredentials = signingCredentials
                    };
                    var token = new JwtSecurityTokenHandler().CreateJwtSecurityToken(tokenDescriptor);
                    return Ok(new JwtSecurityTokenHandler().WriteToken(token));
                }
                return NotFound();
            }
            return BadRequest();
        }
    }
}
