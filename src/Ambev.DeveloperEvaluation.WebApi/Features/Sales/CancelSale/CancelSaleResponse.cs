namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.CancelSale
{
    /// <summary>
    /// Response model for sale cancellation
    /// </summary>
    public class CancelSaleResponse
    {
        /// <summary>
        /// Sale unique identifier
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Sale number
        /// </summary>
        public string SaleNumber { get; set; } = string.Empty;

        /// <summary>
        /// Date when the sale was made
        /// </summary>
        public DateTime SaleDate { get; set; }

        /// <summary>
        /// Date when the sale was cancelled
        /// </summary>
        public DateTime CancelledAt { get; set; }

        /// <summary>
        /// Customer name
        /// </summary>
        public string CustomerName { get; set; } = string.Empty;

        /// <summary>
        /// Branch name
        /// </summary>
        public string BranchName { get; set; } = string.Empty;

        /// <summary>
        /// Total amount of the cancelled sale
        /// </summary>
        public decimal TotalAmount { get; set; }

        /// <summary>
        /// Currency
        /// </summary>
        public string Currency { get; set; } = string.Empty;

        /// <summary>
        /// Sale status (should be "Cancelled")
        /// </summary>
        public string Status { get; set; } = string.Empty;

        /// <summary>
        /// Reason for cancellation
        /// </summary>
        public string? CancellationReason { get; set; }

        /// <summary>
        /// Indicates if the cancellation was successful
        /// </summary>
        public bool WasSuccessfullyCancelled { get; set; }
    }
}
