using Ambev.DeveloperEvaluation.Domain.Enums;
using Ambev.DeveloperEvaluation.Functional.Infrastructure;
using Ambev.DeveloperEvaluation.WebApi.Features.Users.CreateUser;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace Ambev.DeveloperEvaluation.Functional.Controllers
{
    public class UserControllerIntegrationTests : BaseIntegrationTest
    {
        public UserControllerIntegrationTests(CustomWebApplicationFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task CreateUser_ShouldPersistToDatabase()
        {
            // Arrange
            await CleanupDatabaseAsync(); // Limpa dados do teste anterior

            var request = new CreateUserRequest
            {
                Username = "JaumSilva",
                Password = "Teste@123",
                Email = "joao@teste.com",
                Phone = "4136625987",
                Status = (UserStatus)1,
                Role = (UserRole)1
            };

            // Act
            var response = await Client.PostAsJsonAsync("/api/users", request);

            // Assert
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);

            // Verificar no banco usando o contexto
            using var context = await GetDbContextAsync();
            var savedUser = await context.Users.FirstOrDefaultAsync(u => u.Email == "joao@teste.com");

            Assert.NotNull(savedUser);
            Assert.Equal("JaumSilva", savedUser.Username);
        }
    }
}
