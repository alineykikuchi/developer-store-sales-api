namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.CreateSale
{
    /// <summary>
    /// Request model for creating a sale
    /// </summary>
    public class CreateSaleRequest
    {
        /// <summary>
        /// Sale number
        /// </summary>
        public string SaleNumber { get; set; } = string.Empty;

        /// <summary>
        /// Customer information
        /// </summary>
        public CreateSaleCustomerRequest Customer { get; set; } = new();

        /// <summary>
        /// Branch information
        /// </summary>
        public CreateSaleBranchRequest Branch { get; set; } = new();

        /// <summary>
        /// List of items to be sold
        /// </summary>
        public List<CreateSaleItemRequest> Items { get; set; } = new();
    }
}
