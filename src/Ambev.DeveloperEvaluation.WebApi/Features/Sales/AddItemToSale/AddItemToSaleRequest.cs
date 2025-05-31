namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.AddItemToSale
{
    /// <summary>
    /// Request model for adding an item to a sale
    /// </summary>
    public class AddItemToSaleRequest
    {
        /// <summary>
        /// Product information
        /// </summary>
        public AddItemToSaleProductRequest Product { get; set; } = new();

        /// <summary>
        /// Quantity to add
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// Unit price of the product
        /// </summary>
        public decimal UnitPrice { get; set; }

        /// <summary>
        /// Currency (default: BRL)
        /// </summary>
        public string Currency { get; set; } = "BRL";
    }
}
