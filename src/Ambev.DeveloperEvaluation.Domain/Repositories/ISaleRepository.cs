using Ambev.DeveloperEvaluation.Domain.Common;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Enums;

namespace Ambev.DeveloperEvaluation.Domain.Repositories
{
    /// <summary>
    /// Repository interface for Sale entity operations
    /// </summary>
    public interface ISaleRepository
    {
        /// <summary>
        /// Creates a new sale in the repository
        /// </summary>
        /// <param name="sale">The sale to create</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>The created sale</returns>
        Task<Sale> CreateAsync(Sale sale, CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves a sale by its unique identifier
        /// </summary>
        /// <param name="id">The unique identifier of the sale</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>The sale if found, null otherwise</returns>
        Task<Sale?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves a sale by its sale number
        /// </summary>
        /// <param name="saleNumber">The sale number</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>The sale if found, null otherwise</returns>
        Task<Sale?> GetBySaleNumberAsync(string saleNumber, CancellationToken cancellationToken = default);

        /// <summary>
        /// Updates an existing sale in the repository
        /// </summary>
        /// <param name="sale">The sale to update</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>The updated sale</returns>
        Task<Sale> UpdateAsync(Sale sale, CancellationToken cancellationToken = default);

        /// <summary>
        /// Deletes a sale from the repository
        /// </summary>
        /// <param name="id">The unique identifier of the sale to delete</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>True if the sale was deleted, false otherwise</returns>
        Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves sales by customer ID
        /// </summary>
        /// <param name="customerId">The customer unique identifier</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>List of sales for the customer</returns>
        Task<IEnumerable<Sale>> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves sales by branch ID
        /// </summary>
        /// <param name="branchId">The branch unique identifier</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>List of sales for the branch</returns>
        Task<IEnumerable<Sale>> GetByBranchIdAsync(Guid branchId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves sales within a date range
        /// </summary>
        /// <param name="startDate">Start date for the range</param>
        /// <param name="endDate">End date for the range</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>List of sales within the date range</returns>
        Task<IEnumerable<Sale>> GetByDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves active (non-cancelled) sales
        /// </summary>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>List of active sales</returns>
        Task<IEnumerable<Sale>> GetActiveSalesAsync(CancellationToken cancellationToken = default);

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
        Task<PaginatedResult<Sale>> GetPaginatedAsync(
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
            CancellationToken cancellationToken = default);
    }
}
