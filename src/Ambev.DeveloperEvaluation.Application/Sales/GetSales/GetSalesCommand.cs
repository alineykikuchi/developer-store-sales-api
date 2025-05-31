using Ambev.DeveloperEvaluation.Application.Sales.GetSale;
using Ambev.DeveloperEvaluation.Domain.Common;
using Ambev.DeveloperEvaluation.Domain.Enums;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ambev.DeveloperEvaluation.Application.Sales.GetSales
{
    /// <summary>
    /// Command for retrieving sales with pagination and filters
    /// </summary>
    public class GetSalesCommand : IRequest<PaginatedResult<GetSalesResult>>
    {
        /// <summary>
        /// Page number (1-based)
        /// </summary>
        public int Page { get; set; } = 1;

        /// <summary>
        /// Number of items per page
        /// </summary>
        public int PageSize { get; set; } = 10;

        /// <summary>
        /// Filter by customer ID (optional)
        /// </summary>
        public Guid? CustomerId { get; set; }

        /// <summary>
        /// Filter by branch ID (optional)
        /// </summary>
        public Guid? BranchId { get; set; }

        /// <summary>
        /// Filter by sale status (optional)
        /// </summary>
        public SaleStatus? Status { get; set; }

        /// <summary>
        /// Filter by start date (optional)
        /// </summary>
        public DateTime? StartDate { get; set; }

        /// <summary>
        /// Filter by end date (optional)
        /// </summary>
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// Search by sale number (optional)
        /// </summary>
        public string? SaleNumber { get; set; }

        /// <summary>
        /// Search by customer name (optional)
        /// </summary>
        public string? CustomerName { get; set; }

        /// <summary>
        /// Order by field (SaleDate, TotalAmount, SaleNumber)
        /// </summary>
        public string OrderBy { get; set; } = "SaleDate";

        /// <summary>
        /// Order direction (asc, desc)
        /// </summary>
        public string OrderDirection { get; set; } = "desc";
    }
}
