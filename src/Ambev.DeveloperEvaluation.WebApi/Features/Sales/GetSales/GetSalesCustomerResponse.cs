namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.GetSales
{
    /// <summary>
    /// Customer information in sales list response
    /// </summary>
    public class GetSalesCustomerResponse
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
