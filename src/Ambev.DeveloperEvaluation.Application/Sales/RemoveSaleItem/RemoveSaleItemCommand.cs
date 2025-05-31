using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.RemoveSaleItem
{
    /// <summary>
    /// Command for removing an item from an existing sale
    /// </summary>
    public class RemoveSaleItemCommand : IRequest<RemoveSaleItemResult>
    {
        /// <summary>
        /// The unique identifier of the sale
        /// </summary>
        public Guid SaleId { get; set; }

        /// <summary>
        /// The unique identifier of the item to remove
        /// </summary>
        public Guid ItemId { get; set; }

        public RemoveSaleItemCommand(Guid saleId, Guid itemId)
        {
            SaleId = saleId;
            ItemId = itemId;
        }
    }
}
