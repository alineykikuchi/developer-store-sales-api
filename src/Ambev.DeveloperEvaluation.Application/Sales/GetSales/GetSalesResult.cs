using Ambev.DeveloperEvaluation.Domain.Enums;

namespace Ambev.DeveloperEvaluation.Application.Sales.GetSales
{
    /// <summary>
    /// Response for sales list item
    /// </summary>
    public class GetSalesResult
    {
        public Guid Id { get; set; }
        public string SaleNumber { get; set; } = string.Empty;
        public DateTime SaleDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? CancelledAt { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerEmail { get; set; } = string.Empty;
        public Guid CustomerId { get; set; }
        public string BranchName { get; set; } = string.Empty;
        public Guid BranchId { get; set; }
        public decimal TotalAmount { get; set; }
        public string Currency { get; set; } = string.Empty;
        public SaleStatus Status { get; set; }
        public int TotalItemsCount { get; set; }
        public bool HasDiscountedItems { get; set; }
    }
}
