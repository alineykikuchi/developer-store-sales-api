namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.ModifySaleItem
{
    /// <summary>
    /// Request model for modifying a sale item
    /// </summary>
    public class ModifySaleItemRequest
    {
        /// <summary>
        /// New quantity for the item (optional - if not provided, keeps current)
        /// </summary>
        public int? Quantity { get; set; }

        /// <summary>
        /// New unit price for the item (optional - if not provided, keeps current)
        /// </summary>
        public decimal? UnitPrice { get; set; }

        /// <summary>
        /// Currency for the unit price (default: BRL)
        /// </summary>
        public string Currency { get; set; } = "BRL";
    }
}
