namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.ModifySaleItem
{
    /// <summary>
    /// Product information in modified item response
    /// </summary>
    public class ModifySaleItemProductResponse
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
