namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.RemoveSaleItem
{
    /// <summary>
    /// Response model for removing a sale item
    /// </summary>
    public class RemoveSaleItemResponse
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
        /// Details of the removed item
        /// </summary>
        public RemoveSaleItemDetailsResponse RemovedItem { get; set; } = new();

        /// <summary>
        /// New total amount of the sale after removing the item
        /// </summary>
        public decimal NewSaleTotalAmount { get; set; }

        /// <summary>
        /// Currency
        /// </summary>
        public string Currency { get; set; } = string.Empty;

        /// <summary>
        /// Total count of items in the sale after removal
        /// </summary>
        public int TotalItemsCount { get; set; }

        /// <summary>
        /// Indicates if the sale has items with discounts after removal
        /// </summary>
        public bool HasDiscountedItems { get; set; }

        /// <summary>
        /// Indicates if the sale is empty after removal
        /// </summary>
        public bool SaleIsEmpty { get; set; }

        /// <summary>
        /// Date and time when the item was removed
        /// </summary>
        public DateTime UpdatedAt { get; set; }
    }
}
