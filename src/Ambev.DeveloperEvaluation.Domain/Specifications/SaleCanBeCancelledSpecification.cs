using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Enums;

namespace Ambev.DeveloperEvaluation.Domain.Specifications
{
    public class SaleCanBeCancelledSpecification : ISpecification<Sale>
    {
        private readonly int _maxCancellationDays;

        public SaleCanBeCancelledSpecification(int maxCancellationDays = 30)
        {
            _maxCancellationDays = maxCancellationDays;
        }

        public bool IsSatisfiedBy(Sale sale)
        {
            // Business rules for cancellation:
            // 1. Sale must be Active
            // 2. Sale should not be older than 30 days (example business rule)
            // 3. In a real scenario, you might check:
            //    - Not invoiced
            //    - Not shipped
            //    - Not payment processed
            //    - Customer approval, etc.

            
            if (sale.Status != SaleStatus.Active)
                return false;

            var cancellationCutoff = DateTime.UtcNow.Date.AddDays(-_maxCancellationDays);
            return sale.SaleDate.Date >= cancellationCutoff;

            // Add more business rules as needed
            // For example:
            // - Check if sale is invoiced (would require additional properties)
            // - Check if payment is processed
            // - Check if items are shipped

        }
    }
}
