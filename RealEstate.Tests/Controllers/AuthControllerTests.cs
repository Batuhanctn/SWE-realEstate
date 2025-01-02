using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using RealEstate.API.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Json;

namespace RealEstate.Tests.Controllers
{
    /// <summary>
    /// Test suite for AuthController
    /// Contains unit tests for authentication operations
    /// </summary>
    public class AuthControllerTests
    {
        private readonly Mock<IMongoDatabase> _mockDatabase;
        private readonly Mock<IConfiguration> _mockConfiguration;
        private readonly Mock<IConfigurationSection> _mockConfigurationSection;
        private readonly AuthController _controller;
        private readonly Mock<IMongoCollection<User>> _mockCollection;
        private const string TEST_TOKEN_KEY = "my-super-secret-key-for-testing-purposes-only-with-minimum-length";

        private class MessageResponse
        {
            public string message { get; set; } = string.Empty;
        }

        private class LoginResponse
        {
            public string token { get; set; } = string.Empty;
            public UserResponse user { get; set; } = new();
        }

        private class UserResponse
        {
            public string id { get; set; } = string.Empty;
            public string email { get; set; } = string.Empty;
            public string firstName { get; set; } = string.Empty;
            public string lastName { get; set; } = string.Empty;
            public string phoneNumber { get; set; } = string.Empty;
        }

        /// <summary>
        /// Initializes a new instance of AuthControllerTests
        /// Sets up mocks and controller instance for testing
        /// </summary>
        public AuthControllerTests()
        {
            // Setup mock database and collection
            _mockDatabase = new Mock<IMongoDatabase>();
            var mockDb = new MockMongoDb();
            _mockCollection = mockDb.UserCollection;
            _mockDatabase.Setup(d => d.GetCollection<User>("Users", null))
                        .Returns(_mockCollection.Object);

            // Setup mock configuration
            _mockConfiguration = new Mock<IConfiguration>();
            _mockConfigurationSection = new Mock<IConfigurationSection>();
            _mockConfigurationSection.Setup(s => s.Value).Returns(TEST_TOKEN_KEY);
            _mockConfiguration.Setup(c => c.GetSection("AppSettings:Token"))
                            .Returns(_mockConfigurationSection.Object);

            _controller = new AuthController(_mockDatabase.Object, _mockConfiguration.Object);
        }

        /// <summary>
        /// Tests that Register creates new user successfully
        /// </summary>
        [Fact]
        public async Task Register_ValidUser_ReturnsOk()
        {
            // Arrange
            var userDto = new UserDto
            {
                Email = "test@example.com",
                Password = "Test123!",
                FirstName = "Test",
                LastName = "User",
                PhoneNumber = "1234567890"
            };

            var mockCursor = new Mock<IAsyncCursor<User>>();
            mockCursor.Setup(c => c.Current).Returns(new List<User>());
            mockCursor
                .SetupSequence(c => c.MoveNext(It.IsAny<CancellationToken>()))
                .Returns(true)
                .Returns(false);
            mockCursor
                .SetupSequence(c => c.MoveNextAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(true)
                .ReturnsAsync(false);

            _mockCollection.Setup(c => c.FindAsync(
                It.IsAny<FilterDefinition<User>>(),
                It.IsAny<FindOptions<User>>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(mockCursor.Object);

            // Act
            var result = await _controller.Register(userDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var response = JsonSerializer.Deserialize<MessageResponse>(JsonSerializer.Serialize(okResult.Value))!;
            Assert.Equal("User registered successfully", response.message);
        }

        /// <summary>
        /// Tests that Register returns Conflict for duplicate email
        /// </summary>
        [Fact]
        public async Task Register_ExistingEmail_ReturnsBadRequest()
        {
            // Arrange
            var existingUser = new User
            {
                Email = "existing@example.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("password")
            };

            var userDto = new UserDto
            {
                Email = "existing@example.com",
                Password = "Test123!",
                FirstName = "Test",
                LastName = "User"
            };

            var mockCursor = new Mock<IAsyncCursor<User>>();
            mockCursor.Setup(c => c.Current).Returns(new List<User> { existingUser });
            mockCursor
                .SetupSequence(c => c.MoveNext(It.IsAny<CancellationToken>()))
                .Returns(true)
                .Returns(false);
            mockCursor
                .SetupSequence(c => c.MoveNextAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(true)
                .ReturnsAsync(false);

            _mockCollection.Setup(c => c.FindAsync(
                It.IsAny<FilterDefinition<User>>(),
                It.IsAny<FindOptions<User>>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(mockCursor.Object);

            // Act
            var result = await _controller.Register(userDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            var response = JsonSerializer.Deserialize<MessageResponse>(JsonSerializer.Serialize(badRequestResult.Value))!;
            Assert.Equal("User with this email already exists.", response.message);
        }

        /// <summary>
        /// Tests that Register validates required fields
        /// </summary>
        [Fact]
        public async Task Register_InvalidModel_ReturnsBadRequest()
        {
            // Arrange
            var userDto = new UserDto(); // Empty DTO

            // Act
            var result = await _controller.Register(userDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            var response = JsonSerializer.Deserialize<MessageResponse>(JsonSerializer.Serialize(badRequestResult.Value))!;
            Assert.Equal("Email is required", response.message);
        }

        /// <summary>
        /// Tests that Login authenticates user successfully
        /// </summary>
        [Fact]
        public async Task Login_ValidCredentials_ReturnsToken()
        {
            // Arrange
            var password = "Test123!";
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);
            var existingUser = new User
            {
                Id = "test-user-id",
                Email = "test@example.com",
                PasswordHash = hashedPassword,
                FirstName = "Test",
                LastName = "User"
            };

            var loginDto = new LoginDto
            {
                Email = "test@example.com",
                Password = password
            };

            var mockCursor = new Mock<IAsyncCursor<User>>();
            mockCursor.Setup(c => c.Current).Returns(new List<User> { existingUser });
            mockCursor
                .SetupSequence(c => c.MoveNext(It.IsAny<CancellationToken>()))
                .Returns(true)
                .Returns(false);
            mockCursor
                .SetupSequence(c => c.MoveNextAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(true)
                .ReturnsAsync(false);

            _mockCollection.Setup(c => c.FindAsync(
                It.IsAny<FilterDefinition<User>>(),
                It.IsAny<FindOptions<User>>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(mockCursor.Object);

            // Act
            var result = await _controller.Login(loginDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var responseJson = JsonSerializer.Serialize(okResult.Value);
            var response = JsonSerializer.Deserialize<JsonDocument>(responseJson) ?? 
                throw new InvalidOperationException("Failed to deserialize response");
            
            var token = response.RootElement.GetProperty("token").GetString() ?? 
                throw new InvalidOperationException("Token is missing from response");
            var userEmail = response.RootElement.GetProperty("user").GetProperty("email").GetString() ??
                throw new InvalidOperationException("User email is missing from response");

            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);

            // Verify claims
            var claims = jwtToken.Claims.ToList();
            var nameIdClaim = claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            var emailClaim = claims.FirstOrDefault(c => c.Type == ClaimTypes.Email);

            Assert.NotNull(nameIdClaim);
            Assert.NotNull(emailClaim);
            Assert.Equal(existingUser.Id, nameIdClaim.Value);
            Assert.Equal(existingUser.Email, emailClaim.Value);

            // Verify user object
            var userElement = response.RootElement.GetProperty("user");
            Assert.Equal(existingUser.Id, userElement.GetProperty("id").GetString());
            Assert.Equal(existingUser.Email, userElement.GetProperty("email").GetString());
            Assert.Equal(existingUser.FirstName, userElement.GetProperty("firstName").GetString());
            Assert.Equal(existingUser.LastName, userElement.GetProperty("lastName").GetString());
        }

        /// <summary>
        /// Tests that Login returns Unauthorized for invalid credentials
        /// </summary>
        [Fact]
        public async Task Login_InvalidEmail_ReturnsBadRequest()
        {
            // Arrange
            var loginDto = new LoginDto
            {
                Email = "nonexistent@example.com",
                Password = "Test123!"
            };

            var mockCursor = new Mock<IAsyncCursor<User>>();
            mockCursor.Setup(c => c.Current).Returns(new List<User>());
            mockCursor
                .SetupSequence(c => c.MoveNext(It.IsAny<CancellationToken>()))
                .Returns(true)
                .Returns(false);
            mockCursor
                .SetupSequence(c => c.MoveNextAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(true)
                .ReturnsAsync(false);

            _mockCollection.Setup(c => c.FindAsync(
                It.IsAny<FilterDefinition<User>>(),
                It.IsAny<FindOptions<User>>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(mockCursor.Object);

            // Act
            var result = await _controller.Login(loginDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            var response = JsonSerializer.Deserialize<MessageResponse>(JsonSerializer.Serialize(badRequestResult.Value))!;
            Assert.Equal("User not found.", response.message);
        }

        /// <summary>
        /// Tests that Login returns Unauthorized for wrong password
        /// </summary>
        [Fact]
        public async Task Login_WrongPassword_ReturnsBadRequest()
        {
            // Arrange
            var existingUser = new User
            {
                Email = "test@example.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("correctpassword")
            };

            var loginDto = new LoginDto
            {
                Email = "test@example.com",
                Password = "wrongpassword"
            };

            var mockCursor = new Mock<IAsyncCursor<User>>();
            mockCursor.Setup(c => c.Current).Returns(new List<User> { existingUser });
            mockCursor
                .SetupSequence(c => c.MoveNext(It.IsAny<CancellationToken>()))
                .Returns(true)
                .Returns(false);
            mockCursor
                .SetupSequence(c => c.MoveNextAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(true)
                .ReturnsAsync(false);

            _mockCollection.Setup(c => c.FindAsync(
                It.IsAny<FilterDefinition<User>>(),
                It.IsAny<FindOptions<User>>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(mockCursor.Object);

            // Act
            var result = await _controller.Login(loginDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            var response = JsonSerializer.Deserialize<MessageResponse>(JsonSerializer.Serialize(badRequestResult.Value))!;
            Assert.Equal("Wrong password.", response.message);
        }
    }
}
