namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.CreateSale
{
    /// <summary>
    /// Customer information for sale creation
    /// </summary>
    public class CreateSaleCustomerRequest
    {
        /// <summary>
        /// Customer unique identifier
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Customer name
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Customer email
        /// </summary>
        public string Email { get; set; } = string.Empty;
    }
}
