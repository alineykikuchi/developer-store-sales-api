using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Enums;

namespace Ambev.DeveloperEvaluation.Domain.Specifications
{
    public class SaleCanHaveItemRemovedSpecification : ISpecification<Sale>
    {
        public bool IsSatisfiedBy(Sale sale)
        {
            return sale.Items.Count > 1 && sale.Status == SaleStatus.Active;
        }
    }
}
