namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.ModifySaleItem
{
    /// <summary>
    /// Response model for modifying a sale item
    /// </summary>
    public class ModifySaleItemResponse
    {
        /// <summary>
        /// Sale unique identifier
        /// </summary>
        public Guid SaleId { get; set; }

        /// <summary>
        /// Sale number
        /// </summary>
        public string SaleNumber { get; set; } = string.Empty;

        /// <summary>
        /// Details of the modified item
        /// </summary>
        public ModifySaleItemDetailsResponse ModifiedItem { get; set; } = new();

        /// <summary>
        /// New total amount of the sale after modification
        /// </summary>
        public decimal NewSaleTotalAmount { get; set; }

        /// <summary>
        /// Currency
        /// </summary>
        public string Currency { get; set; } = string.Empty;

        /// <summary>
        /// Total count of items in the sale after modification
        /// </summary>
        public int TotalItemsCount { get; set; }

        /// <summary>
        /// Indicates if the sale has items with discounts after modification
        /// </summary>
        public bool HasDiscountedItems { get; set; }

        /// <summary>
        /// Date and time when the item was modified
        /// </summary>
        public DateTime UpdatedAt { get; set; }
    }
}
