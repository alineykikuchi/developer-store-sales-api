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
            if (sale.Status != SaleStatus.Active)
                return false;

            var cancellationCutoff = DateTime.UtcNow.AddDays(-_maxCancellationDays);
            return sale.SaleDate >= cancellationCutoff;
        }
    }
}
