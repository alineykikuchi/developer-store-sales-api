namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.CreateSale
{
    /// <summary>
    /// Response model for sale items
    /// </summary>
    public class CreateSaleItemResponse
    {
        /// <summary>
        /// Item unique identifier
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Product name
        /// </summary>
        public string ProductName { get; set; } = string.Empty;

        /// <summary>
        /// Quantity sold
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// Unit price
        /// </summary>
        public decimal UnitPrice { get; set; }

        /// <summary>
        /// Discount percentage applied
        /// </summary>
        public decimal DiscountPercentage { get; set; }

        /// <summary>
        /// Total amount for this item (after discount)
        /// </summary>
        public decimal TotalAmount { get; set; }
    }

}
