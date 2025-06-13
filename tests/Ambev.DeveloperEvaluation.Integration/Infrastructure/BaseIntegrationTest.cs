using Ambev.DeveloperEvaluation.ORM;
using Xunit;

namespace Ambev.DeveloperEvaluation.Functional.Infrastructure
{
    public abstract class BaseIntegrationTest : IClassFixture<CustomWebApplicationFactory>
    {
        protected readonly HttpClient Client;
        protected readonly CustomWebApplicationFactory Factory;

        protected BaseIntegrationTest(CustomWebApplicationFactory factory)
        {
            Factory = factory;
            Client = factory.CreateClient();
        }

        // Método auxiliar para obter contexto do banco
        protected async Task<DefaultContext> GetDbContextAsync()
        {
            return await Factory.GetDbContextAsync();
        }

        // Método auxiliar para limpar dados entre testes
        protected async Task CleanupDatabaseAsync()
        {
            await Factory.CleanupDatabaseAsync();
        }
    }
}
