namespace Ambev.DeveloperEvaluation.Application.Sales.RemoveSaleItem
{
    /// <summary>
    /// Details of the removed item
    /// </summary>
    public class RemoveSaleItemDetails
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string ProductDescription { get; set; } = string.Empty;
        public int RemovedQuantity { get; set; }
        public decimal RemovedUnitPrice { get; set; }
        public string UnitPriceCurrency { get; set; } = string.Empty;
        public decimal RemovedDiscountPercentage { get; set; }
        public decimal RemovedTotalAmount { get; set; }
        public string TotalAmountCurrency { get; set; } = string.Empty;
        public bool WasSuccessfullyRemoved { get; set; }
    }
}
