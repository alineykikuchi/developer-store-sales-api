namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.CancelSale
{
    /// <summary>
    /// Request model for cancelling a sale
    /// </summary>
    public class CancelSaleRequest
    {
        /// <summary>
        /// Reason for cancelling the sale (optional)
        /// </summary>
        public string? CancellationReason { get; set; }
    }
}
