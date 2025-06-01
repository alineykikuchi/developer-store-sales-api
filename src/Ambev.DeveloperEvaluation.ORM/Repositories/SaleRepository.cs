using Ambev.DeveloperEvaluation.Domain.Common;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Enums;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Ambev.DeveloperEvaluation.ORM.Repositories
{
    public class SaleRepository : ISaleRepository
    {
        private readonly DefaultContext _context;

        /// <summary>
        /// Initializes a new instance of SaleRepository
        /// </summary>
        /// <param name="context">The database context</param>
        public SaleRepository(DefaultContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Creates a new sale in the database
        /// </summary>
        /// <param name="sale">The sale to create</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>The created sale</returns>
        public async Task<Sale> CreateAsync(Sale sale, CancellationToken cancellationToken = default)
        {
            await _context.Sales.AddAsync(sale, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return sale;
        }

        /// <summary>
        /// Retrieves a sale by its unique identifier
        /// </summary>
        /// <param name="id">The unique identifier of the sale</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>The sale if found, null otherwise</returns>
        public async Task<Sale?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.Sales
                .Include(s => s.Items)
                .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
        }

        /// <summary>
        /// Retrieves a sale by its sale number
        /// </summary>
        /// <param name="saleNumber">The sale number</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>The sale if found, null otherwise</returns>
        public async Task<Sale?> GetBySaleNumberAsync(string saleNumber, CancellationToken cancellationToken = default)
        {
            return await _context.Sales
                .Include(s => s.Items)
                .FirstOrDefaultAsync(s => s.SaleNumber == saleNumber, cancellationToken);
        }

        /// <summary>
        /// Updates an existing sale in the database
        /// </summary>
        /// <param name="sale">The sale to update</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>The updated sale</returns>
        public async Task<Sale> UpdateAsync(Sale sale, CancellationToken cancellationToken = default)
        {
            _context.Update(sale);
            await _context.SaveChangesAsync(cancellationToken);
            return sale;
        }

        /// <summary>
        /// Deletes a sale from the database
        /// </summary>
        /// <param name="id">The unique identifier of the sale to delete</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>True if the sale was deleted, false if not found</returns>
        public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var sale = await GetByIdAsync(id, cancellationToken);
            if (sale == null)
                return false;

            _context.Sales.Remove(sale);
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }

        /// <summary>
        /// Retrieves sales by customer ID
        /// </summary>
        /// <param name="customerId">The customer unique identifier</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>List of sales for the customer</returns>
        public async Task<IEnumerable<Sale>> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default)
        {
            return await _context.Sales
                .Include(s => s.Items)
                .Where(s => s.Customer.Id == customerId)
                .OrderByDescending(s => s.SaleDate)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Retrieves sales by branch ID
        /// </summary>
        /// <param name="branchId">The branch unique identifier</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>List of sales for the branch</returns>
        public async Task<IEnumerable<Sale>> GetByBranchIdAsync(Guid branchId, CancellationToken cancellationToken = default)
        {
            return await _context.Sales
                .Include(s => s.Items)
                .Where(s => s.Branch.Id == branchId)
                .OrderByDescending(s => s.SaleDate)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Retrieves sales within a date range
        /// </summary>
        /// <param name="startDate">Start date for the range</param>
        /// <param name="endDate">End date for the range</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>List of sales within the date range</returns>
        public async Task<IEnumerable<Sale>> GetByDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
        {
            return await _context.Sales
                .Include(s => s.Items)
                .Where(s => s.SaleDate >= startDate && s.SaleDate <= endDate)
                .OrderByDescending(s => s.SaleDate)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Retrieves active (non-cancelled) sales
        /// </summary>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>List of active sales</returns>
        public async Task<IEnumerable<Sale>> GetActiveSalesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Sales
                .Include(s => s.Items)
                .Where(s => s.Status == Domain.Enums.SaleStatus.Active)
                .OrderByDescending(s => s.SaleDate)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Retrieves paginated sales with filters and sorting
        /// </summary>
        /// <param name="page">Page number (1-based)</param>
        /// <param name="pageSize">Number of items per page</param>
        /// <param name="customerId">Filter by customer ID (optional)</param>
        /// <param name="branchId">Filter by branch ID (optional)</param>
        /// <param name="status">Filter by sale status (optional)</param>
        /// <param name="startDate">Filter by start date (optional)</param>
        /// <param name="endDate">Filter by end date (optional)</param>
        /// <param name="saleNumber">Search by sale number (optional)</param>
        /// <param name="customerName">Search by customer name (optional)</param>
        /// <param name="orderBy">Order by field</param>
        /// <param name="orderDirection">Order direction (asc/desc)</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Paginated result of sales</returns>
        public async Task<PaginatedResult<Sale>> GetPaginatedAsync(
            int page,
            int pageSize,
            Guid? customerId = null,
            Guid? branchId = null,
            SaleStatus? status = null,
            DateTime? startDate = null,
            DateTime? endDate = null,
            string? saleNumber = null,
            string? customerName = null,
            string orderBy = "SaleDate",
            string orderDirection = "desc",
            CancellationToken cancellationToken = default)
        {
            var query = _context.Sales
                .Include(s => s.Items)
                .AsQueryable();

            // Apply filters
            if (customerId.HasValue)
            {
                query = query.Where(s => s.Customer.Id == customerId.Value);
            }

            if (branchId.HasValue)
            {
                query = query.Where(s => s.Branch.Id == branchId.Value);
            }

            if (status.HasValue)
            {
                query = query.Where(s => s.Status == status.Value);
            }

            if (startDate.HasValue)
            {
                query = query.Where(s => s.SaleDate >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                query = query.Where(s => s.SaleDate <= endDate.Value);
            }

            if (!string.IsNullOrWhiteSpace(saleNumber))
            {
                query = query.Where(s => s.SaleNumber.Contains(saleNumber));
            }

            if (!string.IsNullOrWhiteSpace(customerName))
            {
                query = query.Where(s => s.Customer.Name.Contains(customerName));
            }

            // Apply sorting
            query = orderBy.ToLower() switch
            {
                "salenumber" => orderDirection.ToLower() == "asc"
                    ? query.OrderBy(s => s.SaleNumber)
                    : query.OrderByDescending(s => s.SaleNumber),
                "totalamount" => orderDirection.ToLower() == "asc"
                    ? query.OrderBy(s => s.TotalAmount.Amount)
                    : query.OrderByDescending(s => s.TotalAmount.Amount),
                "customername" => orderDirection.ToLower() == "asc"
                    ? query.OrderBy(s => s.Customer.Name)
                    : query.OrderByDescending(s => s.Customer.Name),
                _ => orderDirection.ToLower() == "asc"
                    ? query.OrderBy(s => s.SaleDate)
                    : query.OrderByDescending(s => s.SaleDate)
            };

            // Get total count
            var totalCount = await query.CountAsync(cancellationToken);

            // Apply pagination
            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return new PaginatedResult<Sale>(items, totalCount, page, pageSize);
        }
    }
}
