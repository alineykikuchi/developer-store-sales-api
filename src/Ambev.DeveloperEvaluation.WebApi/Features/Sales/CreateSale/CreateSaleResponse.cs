namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.CreateSale
{
    /// <summary>
    /// Response model for sale creation
    /// </summary>
    public class CreateSaleResponse
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
        /// Customer name
        /// </summary>
        public string CustomerName { get; set; } = string.Empty;

        /// <summary>
        /// Branch name
        /// </summary>
        public string BranchName { get; set; } = string.Empty;

        /// <summary>
        /// Total amount of the sale
        /// </summary>
        public decimal TotalAmount { get; set; }

        /// <summary>
        /// Currency
        /// </summary>
        public string Currency { get; set; } = string.Empty;

        /// <summary>
        /// Sale status
        /// </summary>
        public string Status { get; set; } = string.Empty;

        /// <summary>
        /// List of sale items
        /// </summary>
        public List<CreateSaleItemResponse> Items { get; set; } = new();
    }
}
