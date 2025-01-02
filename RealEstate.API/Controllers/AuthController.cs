using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using RealEstate.API.Models;
using MongoDB.Driver;
using BCrypt.Net;

namespace RealEstate.API.Controllers
{
    /// <summary>
    /// Controller for handling authentication-related operations
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IMongoCollection<User> _users;
        private readonly IConfiguration _configuration;

        public AuthController(IMongoDatabase database, IConfiguration configuration)
        {
            _users = database.GetCollection<User>("Users");
            _configuration = configuration;
        }

        /// <summary>
        /// Registers a new user
        /// </summary>
        /// <param name="request">User registration details</param>
        /// <returns>Returns authentication result with token if successful</returns>
        /// <response code="200">Returns the authentication result with token</response>
        /// <response code="400">If the registration fails due to validation or existing email</response>
        [HttpPost("register")]
        public async Task<ActionResult<User>> Register([FromBody] UserDto request)
        {
            try
            {
                if (request == null)
                {
                    return BadRequest(new { message = "Request body is empty" });
                }

                if (string.IsNullOrEmpty(request.Email))
                {
                    return BadRequest(new { message = "Email is required" });
                }

                if (string.IsNullOrEmpty(request.Password))
                {
                    return BadRequest(new { message = "Password is required" });
                }

                if (string.IsNullOrEmpty(request.FirstName))
                {
                    return BadRequest(new { message = "First name is required" });
                }

                if (string.IsNullOrEmpty(request.LastName))
                {
                    return BadRequest(new { message = "Last name is required" });
                }

                // Check if user already exists
                var existingUser = await _users.Find(u => u.Email == request.Email).FirstOrDefaultAsync();
                if (existingUser != null)
                {
                    return BadRequest(new { message = "User with this email already exists." });
                }

                // Hash password
                string passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

                // Create new user
                var user = new User
                {
                    Email = request.Email,
                    PasswordHash = passwordHash,
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    PhoneNumber = request.PhoneNumber,
                    CreatedAt = DateTime.UtcNow
                };

                // Insert user to database
                await _users.InsertOneAsync(user);

                return Ok(new { message = "User registered successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Internal server error: {ex.Message}" });
            }
        }

        /// <summary>
        /// Authenticates a user and returns a JWT token
        /// </summary>
        /// <param name="request">User login credentials</param>
        /// <returns>Returns authentication result with token if successful</returns>
        /// <response code="200">Returns the authentication result with token</response>
        /// <response code="401">If the login credentials are invalid</response>
        [HttpPost("login")]
        public async Task<ActionResult<string>> Login([FromBody] LoginDto request)
        {
            // Find user by email
            var user = await _users.Find(u => u.Email == request.Email).FirstOrDefaultAsync();
            if (user == null)
            {
                return BadRequest(new { message = "User not found." });
            }

            // Verify password
            if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                return BadRequest(new { message = "Wrong password." });
            }

            // Generate JWT token
            string token = CreateToken(user);

            var response = new
            {
                token = token,
                user = new
                {
                    id = user.Id,
                    email = user.Email,
                    firstName = user.FirstName,
                    lastName = user.LastName,
                    phoneNumber = user.PhoneNumber
                }
            };

            return Ok(response);
        }

        private string CreateToken(User user)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.GivenName, user.FirstName),
                new Claim(ClaimTypes.Surname, user.LastName)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                _configuration.GetSection("AppSettings:Token").Value!));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds);

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;
        }
    }
}
