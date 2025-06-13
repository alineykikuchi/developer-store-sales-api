using Ambev.DeveloperEvaluation.ORM;
using Ambev.DeveloperEvaluation.WebApi;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Testcontainers.PostgreSql;
using Xunit;
using Ambev.DeveloperEvaluation.Domain.Entities;

namespace Ambev.DeveloperEvaluation.Functional.Infrastructure;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _dbContainer = new PostgreSqlBuilder()
        .WithImage("postgres:15")
        .WithDatabase("testdb")
        .WithUsername("testuser")
        .WithPassword("testpass")
        .WithCleanUp(true)
        .Build();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Remove o DbContext original se existir
            services.RemoveAll(typeof(DbContextOptions<DefaultContext>));

            // Adiciona o DbContext com PostgreSQL do container
            services.AddDbContext<DefaultContext>(options =>
            {
                options.UseNpgsql(_dbContainer.GetConnectionString());
                options.EnableSensitiveDataLogging(); 
            });
        });

        // Configurações específicas para testes
        builder.ConfigureAppConfiguration((context, config) =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string>
            {
                ["ConnectionStrings:DefaultConnection"] = _dbContainer.GetConnectionString(),
                ["Logging:LogLevel:Default"] = "Information",
                ["Logging:LogLevel:Microsoft.EntityFrameworkCore"] = "Warning"
            }!);
        });

        builder.UseEnvironment("Testing");
    }

    // IAsyncLifetime
    public async Task InitializeAsync()
    {
        Console.WriteLine("Iniciando container PostgreSQL para testes...");

        // Inicia o container PostgreSQL
        await _dbContainer.StartAsync();

        Console.WriteLine($"Container PostgreSQL iniciado: {_dbContainer.GetConnectionString()}");

        // Aplicar schema/migrations ao banco
        using var scope = Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<DefaultContext>();

        Console.WriteLine("Criando schema do banco de dados...");
        await context.Database.EnsureCreatedAsync();


        Console.WriteLine("Schema criado com sucesso!");

        // Seed de dados comum para todos os testes (opcional)
        await SeedCommonTestDataAsync(context);
    }

    public new async Task DisposeAsync()
    {
        Console.WriteLine("Parando e removendo container PostgreSQL...");

        await _dbContainer.StopAsync();
        await _dbContainer.DisposeAsync();
        await base.DisposeAsync();

        Console.WriteLine("Container removido com sucesso!");
    }

    private async Task SeedCommonTestDataAsync(DefaultContext context)
    {
        // Adicione dados que devem existir em todos os testes
        // Por exemplo, usuários admin, configurações básicas, etc.

        if (!await context.Users.AnyAsync())
        {
            Console.WriteLine("Executando seed de dados de teste...");

            // Exemplo de dados iniciais
            context.Users.AddRange(
                new User { Username = "Admin User", Email = "admin@test.com" },
                new User { Username = "Regular User", Email = "user@test.com" }
            );

            await context.SaveChangesAsync();
            Console.WriteLine("Seed de dados concluído!");
        }
    }

    // Método auxiliar para obter contexto do banco (usado nos testes)
    public async Task<DefaultContext> GetDbContextAsync()
    {
        var scope = Services.CreateScope();
        return scope.ServiceProvider.GetRequiredService<DefaultContext>();
    }

    // Método auxiliar para limpar dados entre testes
    public async Task CleanupDatabaseAsync()
    {
        using var context = await GetDbContextAsync();

        // Remove todos os dados exceto dados de seed
        var usersToRemove = context.Users
            .Where(u => u.Email != "admin@test.com" && u.Email != "user@test.com");

        context.Users.RemoveRange(usersToRemove);
        await context.SaveChangesAsync();
    }
}