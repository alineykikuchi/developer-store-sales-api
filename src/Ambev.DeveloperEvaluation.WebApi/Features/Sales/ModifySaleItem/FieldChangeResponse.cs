namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.ModifySaleItem
{
    /// <summary>
    /// Generic field change information
    /// </summary>
    public class FieldChangeResponse<T>
    {
        /// <summary>
        /// Previous value
        /// </summary>
        public T Previous { get; set; } = default!;

        /// <summary>
        /// New value
        /// </summary>
        public T New { get; set; } = default!;

        /// <summary>
        /// Indicates if the field was changed
        /// </summary>
        public bool Changed { get; set; }
    }
}
