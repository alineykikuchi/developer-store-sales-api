namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.RemoveSaleItem
{
    /// <summary>
    /// Product information of the removed item
    /// </summary>
    public class RemoveSaleItemProductResponse
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
