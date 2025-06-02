using Ambev.DeveloperEvaluation.Domain.Entities;

namespace Ambev.DeveloperEvaluation.Domain.Specifications
{
    public class ProductExistsInSaleSpecification : ISpecification<Sale>
    {
        private readonly Guid _productId;

        public ProductExistsInSaleSpecification(Guid productId)
        {
            _productId = productId;
        }

        public bool IsSatisfiedBy(Sale sale)
        {
            if (sale?.Items == null || !sale.Items.Any())
                return false;

            return sale.Items.Any(item => item?.Product?.Id == _productId);
        }
    }
}
