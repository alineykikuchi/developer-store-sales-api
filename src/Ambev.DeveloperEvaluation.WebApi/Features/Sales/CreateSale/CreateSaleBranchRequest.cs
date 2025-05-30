namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.CreateSale
{
    /// <summary>
    /// Branch information for sale creation
    /// </summary>
    public class CreateSaleBranchRequest
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
