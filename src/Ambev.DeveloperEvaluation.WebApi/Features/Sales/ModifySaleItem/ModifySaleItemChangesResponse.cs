namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.ModifySaleItem
{
    /// <summary>
    /// Changes made to the item
    /// </summary>
    public class ModifySaleItemChangesResponse
    {
        /// <summary>
        /// Quantity change information
        /// </summary>
        public FieldChangeResponse<int> Quantity { get; set; } = new();

        /// <summary>
        /// Unit price change information
        /// </summary>
        public FieldChangeResponse<decimal> UnitPrice { get; set; } = new();

        /// <summary>
        /// Discount percentage change information
        /// </summary>
        public FieldChangeResponse<decimal> DiscountPercentage { get; set; } = new();

        /// <summary>
        /// Total amount change information
        /// </summary>
        public FieldChangeResponse<decimal> TotalAmount { get; set; } = new();
    }
}
