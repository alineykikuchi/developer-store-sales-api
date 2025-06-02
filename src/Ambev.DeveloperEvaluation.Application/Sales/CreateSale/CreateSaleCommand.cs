using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.CreateSale
{
    /// <summary>
    /// Command for creating a new sale
    /// </summary>
    public class CreateSaleCommand : IRequest<CreateSaleResult>
    {
        /// <summary>
        /// Sale number
        /// </summary>
        public string SaleNumber { get; set; } = string.Empty;

        /// <summary>
        /// Customer information
        /// </summary>
        public CreateSaleCustomer Customer { get; set; } = new();

        /// <summary>
        /// Branch information
        /// </summary>
        public CreateSaleBranch Branch { get; set; } = new();

        /// <summary>
        /// List of items to be sold
        /// </summary>
        public List<CreateSaleItem> Items { get; set; } = new();
    }
}
