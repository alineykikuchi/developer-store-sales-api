namespace Ambev.DeveloperEvaluation.Application.Sales.RemoveSaleItem
{
    /// <summary>
    /// Response after removing an item from sale
    /// </summary>
    public class RemoveSaleItemResult
    {
        public Guid SaleId { get; set; }
        public string SaleNumber { get; set; } = string.Empty;
        public RemoveSaleItemDetails RemovedItem { get; set; } = new();
        public decimal NewSaleTotalAmount { get; set; }
        public string Currency { get; set; } = string.Empty;
        public int TotalItemsCount { get; set; }
        public bool HasDiscountedItems { get; set; }
        public DateTime UpdatedAt { get; set; }
        public bool SaleIsEmpty { get; set; }
    }
}
