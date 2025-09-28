using api.Dtos.Login;
using api.Interfaces;
using api.Models;
using gymappforbigmuscle.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace api.Controllers
{
    [ApiController]
    [Route("api/login")]
    public class LoginController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _config;

        public LoginController(IUserRepository userRepository, IConfiguration config)
        {
            _userRepository = userRepository;
            _config = config;
        }

        [HttpPost]
        public async Task<IActionResult> LoginAsync([FromBody] LoginRequestDto request)
        {
            var user = await _userRepository.GetByEmailAsync(request.Email);

           // Console.WriteLine(user != null ? $"User found: {user.Email}" : "User not found");

            if (user == null)
                return Unauthorized();


            var hasher = new PasswordHasher<User>();
            var result = hasher.VerifyHashedPassword(user, user.Password, request.Password);

         //   Console.WriteLine($"Password verification result: {result}"); 

            if (result != PasswordVerificationResult.Success)
                return Unauthorized();

            var role = user.Role != null ? user.Role.Name : "user";
            var username = user.Name != null ? user.Name : "";

            var token = GenerateJwtToken(user);
           // Console.WriteLine($"Generated JWT token: {token}");
            return Ok(new { Token = token });
        }

        private string GenerateJwtToken(User user)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.Name ?? ""),
               new Claim(ClaimTypes.Role, user.RoleId.ToString()),

        };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}