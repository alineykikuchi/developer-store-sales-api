namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.ModifySaleItem
{
    /// <summary>
    /// Details of the modified item in response
    /// </summary>
    public class ModifySaleItemDetailsResponse
    {
        /// <summary>
        /// Item unique identifier
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Product information
        /// </summary>
        public ModifySaleItemProductResponse Product { get; set; } = new();

        /// <summary>
        /// Changes made to the item
        /// </summary>
        public ModifySaleItemChangesResponse Changes { get; set; } = new();

        /// <summary>
        /// Current state after modification
        /// </summary>
        public ModifySaleItemCurrentStateResponse CurrentState { get; set; } = new();
    }
}
