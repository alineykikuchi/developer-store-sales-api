namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.AddItemToSale
{
    /// <summary>
    /// Response model for adding item to sale
    /// </summary>
    public class AddItemToSaleResponse
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
        /// Information about the added item
        /// </summary>
        public AddItemToSaleItemResponse AddedItem { get; set; } = new();

        /// <summary>
        /// New total amount of the sale after adding the item
        /// </summary>
        public decimal NewSaleTotalAmount { get; set; }

        /// <summary>
        /// Currency
        /// </summary>
        public string Currency { get; set; } = string.Empty;

        /// <summary>
        /// Total count of items in the sale after addition
        /// </summary>
        public int TotalItemsCount { get; set; }

        /// <summary>
        /// Indicates if the sale has items with discounts after addition
        /// </summary>
        public bool HasDiscountedItems { get; set; }

        /// <summary>
        /// Date and time when the item was added
        /// </summary>
        public DateTime UpdatedAt { get; set; }
    }
}
