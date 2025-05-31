namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.AddItemToSale
{
    /// <summary>
    /// Product information for adding item to sale request
    /// </summary>
    public class AddItemToSaleProductRequest
    {
        /// <summary>
        /// Product unique identifier
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Product name
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Product description
        /// </summary>
        public string Description { get; set; } = string.Empty;
    }
}
