namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.RemoveSaleItem
{
    /// <summary>
    /// Removed item details in response
    /// </summary>
    public class RemoveSaleItemDetailsResponse
    {
        /// <summary>
        /// Item unique identifier that was removed
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Product information of the removed item
        /// </summary>
        public RemoveSaleItemProductResponse Product { get; set; } = new();

        /// <summary>
        /// Summary of what was removed
        /// </summary>
        public RemoveSaleItemSummaryResponse Summary { get; set; } = new();

        /// <summary>
        /// Indicates if the item was successfully removed
        /// </summary>
        public bool WasSuccessfullyRemoved { get; set; }
    }
}
