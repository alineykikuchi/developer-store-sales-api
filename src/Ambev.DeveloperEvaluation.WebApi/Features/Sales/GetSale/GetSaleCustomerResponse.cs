namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.GetSale
{
    /// <summary>
    /// Customer information in sale response
    /// </summary>
    public class GetSaleCustomerResponse
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
