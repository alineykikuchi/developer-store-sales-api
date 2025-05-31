using Ambev.DeveloperEvaluation.Domain.Entities;

namespace Ambev.DeveloperEvaluation.Domain.Repositories
{
    public interface ISaleItemRepository
    {
        Task AddAsync(SaleItem item, CancellationToken cancellationToken = default);
        Task RemoveAsync(SaleItem item, CancellationToken cancellationToken = default);
    }
}
