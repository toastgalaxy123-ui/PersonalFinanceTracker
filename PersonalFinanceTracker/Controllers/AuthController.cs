using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PersonalFinanceTracker.Models.DTOs;
using PersonalFinanceTracker.Models;

using Microsoft.Extensions.Configuration; // Used for JWT Secret Key access
using System.Threading.Tasks;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;

namespace PersonalFinanceTracker.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly IConfiguration _configuration;

        //Dependency Injection for Identity and JWT
        public AuthController(
            UserManager<User> userManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _configuration = configuration;
        }

        //POST api/auth/register
        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> Register([FromBody] UserRegistrationDto model)
        { // 1. Basic Validation (checks annotations like [Required] in the DTO)
            if (!ModelState.IsValid)
            {
                return BadRequest(new AuthResultDto
                {
                    IsSuccess = false,
                    Errors = new List<string> { "Invalid payload for registration." }
                });
            }

            // 2. Check if User Already Exists
            var userExists = await _userManager.FindByEmailAsync(model.Email);
            if (userExists != null)
            {
                return BadRequest(new AuthResultDto
                {
                    IsSuccess = false,
                    Errors = new List<string> { "User with this email already exists." }
                });
            }

            // 3. Create the New User
            var newUser = new User
            {
                Email = model.Email,
                UserName = model.Email, // Often set UserName to Email for simplicity
                FirstName = model.FirstName,
                LastName = model.LastName
            };

            var result = await _userManager.CreateAsync(newUser, model.Password);

            // 4. Handle Identity Creation Result
            if (result.Succeeded)
            {
                // If creation succeeds, generate the token and return success
                var token = await GenerateJwtToken(newUser);

                return Ok(new AuthResultDto
                {
                    IsSuccess = true,
                    Token = token,
                    DisplayName = newUser.FirstName
                });
            }

            // If creation failed (e.g., password didn't meet complexity requirements)
            return BadRequest(new AuthResultDto
            {
                IsSuccess = false,
                Errors = result.Errors.Select(e => e.Description).ToList()
            });
        }
        // -------------------------------------------------------------------
        // POST: api/Auth/Login
        // -------------------------------------------------------------------

        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login([FromBody] UserLoginDto model)
        {
            // 1. Basic Validation
            if (!ModelState.IsValid)
            {
                return BadRequest(new AuthResultDto
                {
                    IsSuccess = false,
                    Errors = new List<string> { "Invalid login payload." }
                });
            }

            // 2. Find User and Check Password
            var existingUser = await _userManager.FindByEmailAsync(model.Email);
            if (existingUser == null)
            {
                return Unauthorized(new AuthResultDto
                {
                    IsSuccess = false,
                    Errors = new List<string> { "Invalid authentication request." }
                });
            }

            var isCorrectPassword = await _userManager.CheckPasswordAsync(existingUser, model.Password);

            // 3. Check Result and Issue Token
            if (isCorrectPassword)
            {
                var token = await GenerateJwtToken(existingUser);

                return Ok(new AuthResultDto
                {
                    IsSuccess = true,
                    Token = token,
                    DisplayName = existingUser.FirstName
                });
            }

            // Authentication failed
            return Unauthorized(new AuthResultDto
            {
                IsSuccess = false,
                Errors = new List<string> { "Invalid authentication request." }
            });
        }
        

        // Add this private method to the AuthController class
        private async Task<string> GenerateJwtToken(User user)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var secret = jwtSettings.GetValue<string>("Secret");
            var issuer = jwtSettings.GetValue<string>("Issuer");
            var audience = jwtSettings.GetValue<string>("Audience");

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Email, user.Email),
                new Claim(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("firstName", user.FirstName ?? string.Empty),
                new Claim("lastName", user.LastName ?? string.Empty)
            };

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddDays(7),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
