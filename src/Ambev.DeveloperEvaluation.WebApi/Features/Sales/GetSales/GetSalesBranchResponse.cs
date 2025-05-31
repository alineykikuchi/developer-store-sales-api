namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.GetSales
{
    /// <summary>
    /// Branch information in sales list response
    /// </summary>
    public class GetSalesBranchResponse
    {
        /// <summary>
        /// Branch unique identifier
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Branch name
        /// </summary>
        public string Name { get; set; } = string.Empty;
    }
}
