using Ambev.DeveloperEvaluation.Domain.Entities;

namespace Ambev.DeveloperEvaluation.Domain.Specifications
{
    public class ItemExistsInSaleSpecification : ISpecification<Sale>
    {
        private readonly Guid _itemId;

        public ItemExistsInSaleSpecification(Guid itemId)
        {
            _itemId = itemId;
        }

        public bool IsSatisfiedBy(Sale sale)
        {
            return sale.Items.Any(item => item.Id == _itemId);
        }
    }
}
