namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.ModifySaleItem
{
    /// <summary>
    /// Current state of the item after modification
    /// </summary>
    public class ModifySaleItemCurrentStateResponse
    {
        /// <summary>
        /// Current quantity
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// Current unit price
        /// </summary>
        public decimal UnitPrice { get; set; }

        /// <summary>
        /// Current unit price currency
        /// </summary>
        public string UnitPriceCurrency { get; set; } = string.Empty;

        /// <summary>
        /// Current discount percentage applied
        /// </summary>
        public decimal DiscountPercentage { get; set; }

        /// <summary>
        /// Current total amount for this item (after discount)
        /// </summary>
        public decimal TotalAmount { get; set; }

        /// <summary>
        /// Current total amount currency
        /// </summary>
        public string TotalAmountCurrency { get; set; } = string.Empty;
    }

}
