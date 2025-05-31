namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.GetSale
{
    /// <summary>
    /// Sale item information in response
    /// </summary>
    public class GetSaleItemResponse
    {
        /// <summary>
        /// Item unique identifier
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Product information
        /// </summary>
        public GetSaleProductResponse Product { get; set; } = new();

        /// <summary>
        /// Quantity sold
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
