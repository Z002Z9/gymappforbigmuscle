using api.Data;
using api.Dtos.Login;
using api.Dtos.TokenUpdate;
using api.Interfaces;
using api.Models;
using gymappforbigmuscle.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace api.Controllers
{
    [ApiController]
    [Route("api/login")]
    public class LoginController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _config;

        private readonly ApplicationDBContext _db;

        public LoginController(IUserRepository userRepository, IConfiguration config, ApplicationDBContext db)
        {
            _userRepository = userRepository;
            _config = config;
            _db = db; //adatbáizishoz kell a token update miatt
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
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),

        };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddSeconds(30),
                signingCredentials: creds
            );
            Console.WriteLine($"Token expires at (Local): {token.ValidTo.ToLocalTime()}");
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        [HttpPost("TokenUpdate")]
          public async Task<TokenUpdater> GenerateRefreshToken(string userId)
          {
              // JWT létrehozása
              var key = Encoding.UTF8.GetBytes(_config["Jwt:Key"]);
              var tokenHandler = new JwtSecurityTokenHandler();

              var claims = new[]
              {
                  new Claim(ClaimTypes.NameIdentifier, userId)
              };

              var tokenDescriptor = new SecurityTokenDescriptor
              {
                  Subject = new ClaimsIdentity(claims),
                  Expires = DateTime.UtcNow.AddHours(1),
                  SigningCredentials = new SigningCredentials(
                      new SymmetricSecurityKey(key),
                      SecurityAlgorithms.HmacSha256Signature)
              };

              var token = tokenHandler.CreateToken(tokenDescriptor);
              var jwtTokenString = tokenHandler.WriteToken(token);

              var tokenUpdater = new TokenUpdater
              {
                  UserId = userId,
                  Token = jwtTokenString,
                  Expires = tokenDescriptor.Expires.Value,
                  Expired = false
              };


              Console.WriteLine($"Refresh token expires at: {tokenUpdater.Expires.ToLocalTime()}");

              _db.TokenUpdate.Add(tokenUpdater);
              await _db.SaveChangesAsync();

              return tokenUpdater;
          }


          [HttpPost("refresh")]
          public async Task<IActionResult> Refresh([FromBody] TokenUpdateRequestDto request)
          {
              if (string.IsNullOrEmpty(request.TokenUpdate))
                  return BadRequest("Refresh token is required.");

            //db-ből kiszedjük az update tokent 
              var tokenRecord = await _db.TokenUpdate
                  .FirstOrDefaultAsync(t => t.Token == request.TokenUpdate);

              if (tokenRecord == null)
                  return Unauthorized("Invalid refresh token.");

              if (tokenRecord.Expired || tokenRecord.Expires < DateTime.UtcNow)
                  return Unauthorized("Expired refresh token.");

            //lekérdezzük a usert
              var userIdInt = int.Parse(tokenRecord.UserId);
              var user = await _userRepository.GetByIdAsync(userIdInt);
              if (user == null)
                  return Unauthorized("User not found.");

            //új access token
              var accessToken = GenerateJwtToken(user); 

              tokenRecord.Expired = true;

              var newRefreshToken = await GenerateRefreshToken(user.Id.ToString()); 
              await _db.SaveChangesAsync();

              return Ok(new
              {
                  AccessToken = accessToken,
                  RefreshToken = newRefreshToken.Token
              });
          }
        




    }
}