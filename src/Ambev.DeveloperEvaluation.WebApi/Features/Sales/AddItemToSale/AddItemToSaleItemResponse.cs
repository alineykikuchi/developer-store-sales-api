namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.AddItemToSale
{
    /// <summary>
    /// Added item information in response
    /// </summary>
    public class AddItemToSaleItemResponse
    {
        /// <summary>
        /// Item unique identifier
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Product information
        /// </summary>
        public AddItemToSaleProductResponse Product { get; set; } = new();

        /// <summary>
        /// Quantity added
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// Unit price
        /// </summary>
        public decimal UnitPrice { get; set; }

        /// <summary>
        /// Unit price currency
        /// </summary>
        public string UnitPriceCurrency { get; set; } = string.Empty;

        /// <summary>
        /// Discount percentage applied
        /// </summary>
        public decimal DiscountPercentage { get; set; }

        /// <summary>
        /// Total amount for this item (after discount)
        /// </summary>
        public decimal TotalAmount { get; set; }

        /// <summary>
        /// Total amount currency
        /// </summary>
        public string TotalAmountCurrency { get; set; } = string.Empty;
    }
}
