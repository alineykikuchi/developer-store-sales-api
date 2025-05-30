namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.CreateSale
{
    /// <summary>
    /// Item information for sale creation
    /// </summary>
    public class CreateSaleItemRequest
    {
        /// <summary>
        /// Product unique identifier
        /// </summary>
        public Guid ProductId { get; set; }

        /// <summary>
        /// Product name
        /// </summary>
        public string ProductName { get; set; } = string.Empty;

        /// <summary>
        /// Product description
        /// </summary>
        public string ProductDescription { get; set; } = string.Empty;

        /// <summary>
        /// Quantity to be sold
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
