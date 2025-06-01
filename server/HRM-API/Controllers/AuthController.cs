using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using HRM_API.DTOs;
using HRM_API.Model;
using HRM_API.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace HRM_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserRepository userRepository;
        private readonly IOptions<JwtSettings> jwtSettings;
        public AuthController(UserRepository userRepository, IOptions<JwtSettings> jwtSettings)
        {
            this.userRepository = userRepository;
            this.jwtSettings = jwtSettings;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO loginDTO)
        {
            if (string.IsNullOrWhiteSpace(loginDTO.Username))
            {
                return BadRequest("Username is required");
            }

            var user = await userRepository.GetUserByUsername(loginDTO.Username);


            if (user == null)
            {
                return Unauthorized("Invalid username or password");
            }
            var hasher = new PasswordHasher<User>();
            var result = hasher.VerifyHashedPassword(user, user.Password, loginDTO.Password);
            if (result == PasswordVerificationResult.Failed)
            {
                return Unauthorized("Invalid username or password");
            }

            var token = GenerateJwtToken(user, jwtSettings.Value);

            return Ok(new { token, username = user.Username, role = user.RoleId });
        }

        private string GenerateJwtToken(User user, JwtSettings settings)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(settings.Key);

            var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Role, user.RoleId.ToString())
        };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(settings.ExpireMinutes),
                Issuer = settings.Issuer,
                Audience = settings.Audience,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
