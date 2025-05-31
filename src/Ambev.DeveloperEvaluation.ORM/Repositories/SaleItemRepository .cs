using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;

namespace Ambev.DeveloperEvaluation.ORM.Repositories
{
    public class SaleItemRepository : ISaleItemRepository
    {
        private readonly DefaultContext _context;

        public SaleItemRepository(DefaultContext context)
        {
            _context = context;
        }

        public async Task AddAsync(SaleItem item, CancellationToken cancellationToken = default)
        {
            await _context.SaleItems.AddAsync(item, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task RemoveAsync(SaleItem item, CancellationToken cancellationToken = default)
        {
            _context.SaleItems.Remove(item);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
