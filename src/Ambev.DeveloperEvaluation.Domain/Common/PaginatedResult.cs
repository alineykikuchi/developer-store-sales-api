using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ambev.DeveloperEvaluation.Domain.Common
{
    /// <summary>
    /// Represents a paginated result for domain operations
    /// </summary>
    /// <typeparam name="T">The type of items in the paginated result</typeparam>
    public class PaginatedResult<T>
    {
        /// <summary>
        /// The items in the current page
        /// </summary>
        public IEnumerable<T> Items { get; private set; }

        /// <summary>
        /// Current page number (1-based)
        /// </summary>
        public int CurrentPage { get; private set; }

        /// <summary>
        /// Total number of pages
        /// </summary>
        public int TotalPages { get; private set; }

        /// <summary>
        /// Number of items per page
        /// </summary>
        public int PageSize { get; private set; }

        /// <summary>
        /// Total number of items across all pages
        /// </summary>
        public int TotalCount { get; private set; }

        /// <summary>
        /// Indicates if there's a previous page
        /// </summary>
        public bool HasPrevious => CurrentPage > 1;

        /// <summary>
        /// Indicates if there's a next page
        /// </summary>
        public bool HasNext => CurrentPage < TotalPages;

        /// <summary>
        /// Initializes a new instance of PaginatedResult
        /// </summary>
        /// <param name="items">The items in the current page</param>
        /// <param name="totalCount">Total number of items</param>
        /// <param name="currentPage">Current page number (1-based)</param>
        /// <param name="pageSize">Number of items per page</param>
        public PaginatedResult(IEnumerable<T> items, int totalCount, int currentPage, int pageSize)
        {
            Items = items ?? throw new ArgumentNullException(nameof(items));
            TotalCount = totalCount;
            PageSize = pageSize;
            CurrentPage = currentPage;
            TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
        }

        /// <summary>
        /// Creates an empty paginated result
        /// </summary>
        /// <param name="currentPage">Current page number</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Empty paginated result</returns>
        public static PaginatedResult<T> Empty(int currentPage = 1, int pageSize = 10)
        {
            return new PaginatedResult<T>(Enumerable.Empty<T>(), 0, currentPage, pageSize);
        }

        /// <summary>
        /// Maps the items to a different type
        /// </summary>
        /// <typeparam name="TResult">Target type</typeparam>
        /// <param name="mapper">Mapping function</param>
        /// <returns>Paginated result with mapped items</returns>
        public PaginatedResult<TResult> Map<TResult>(Func<T, TResult> mapper)
        {
            var mappedItems = Items.Select(mapper);
            return new PaginatedResult<TResult>(mappedItems, TotalCount, CurrentPage, PageSize);
        }
    }
}
