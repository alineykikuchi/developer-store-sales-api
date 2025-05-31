namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.GetSale
{
    /// <summary>
    /// Response model for retrieving a sale
    /// </summary>
    public class GetSaleResponse
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
        /// Date when the sale was created
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Date when the sale was cancelled (if applicable)
        /// </summary>
        public DateTime? CancelledAt { get; set; }

        /// <summary>
        /// Customer information
        /// </summary>
        public GetSaleCustomerResponse Customer { get; set; } = new();

        /// <summary>
        /// Branch information
        /// </summary>
        public GetSaleBranchResponse Branch { get; set; } = new();

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
        public List<GetSaleItemResponse> Items { get; set; } = new();

        /// <summary>
        /// Total count of items in the sale
        /// </summary>
        public int TotalItemsCount { get; set; }

        /// <summary>
        /// Indicates if the sale has items with discounts
        /// </summary>
        public bool HasDiscountedItems { get; set; }
    }
}
