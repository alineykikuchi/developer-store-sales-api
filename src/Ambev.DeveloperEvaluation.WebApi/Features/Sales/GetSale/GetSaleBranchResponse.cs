namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.GetSale
{
    /// <summary>
    /// Branch information in sale response
    /// </summary>
    public class GetSaleBranchResponse
    {
        /// <summary>
        /// Branch unique identifier
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Branch name
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Branch address
        /// </summary>
        public string Address { get; set; } = string.Empty;
    }

}
