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
            return sale.Items.Any(item => item.Product.Id == _productId);
        }
    }
}
