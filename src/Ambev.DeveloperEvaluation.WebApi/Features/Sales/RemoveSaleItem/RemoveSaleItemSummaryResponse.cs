namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.RemoveSaleItem
{
    /// <summary>
    /// Summary of what was removed
    /// </summary>
    public class RemoveSaleItemSummaryResponse
    {
        /// <summary>
        /// Quantity that was removed
        /// </summary>
        public int RemovedQuantity { get; set; }

        /// <summary>
        /// Unit price of the removed item
        /// </summary>
        public decimal RemovedUnitPrice { get; set; }

        /// <summary>
        /// Unit price currency
        /// </summary>
        public string UnitPriceCurrency { get; set; } = string.Empty;

        /// <summary>
        /// Discount percentage that was applied to the removed item
        /// </summary>
        public decimal RemovedDiscountPercentage { get; set; }

        /// <summary>
        /// Total amount that was deducted from the sale
        /// </summary>
        public decimal RemovedTotalAmount { get; set; }

        /// <summary>
        /// Total amount currency
        /// </summary>
        public string TotalAmountCurrency { get; set; } = string.Empty;
    }
}
