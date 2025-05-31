using Ambev.DeveloperEvaluation.Domain.Common;
using Ambev.DeveloperEvaluation.Domain.ValueObjects;

namespace Ambev.DeveloperEvaluation.Domain.Entities
{
    public class SaleItem : BaseEntity
    {
        public ProductId Product { get; private set; }
        public int Quantity { get; private set; }
        public Money UnitPrice { get; private set; }
        public decimal DiscountPercentage { get; private set; }
        public Money TotalAmount { get; private set; }

        protected SaleItem() { }
        public SaleItem(ProductId product, int quantity, Money unitPrice)
        {
            Id = Guid.Empty;
            Product = product ?? throw new ArgumentNullException(nameof(product));
            SetQuantity(quantity);
            UnitPrice = unitPrice ?? throw new ArgumentNullException(nameof(unitPrice));

            ApplyDiscountRules();
            CalculateTotalAmount();
        }

        private void SetQuantity(int quantity)
        {
            if (quantity <= 0)
                throw new ArgumentException("Quantity must be greater than zero", nameof(quantity));

            if (quantity > 20)
                throw new ArgumentException("Cannot sell more than 20 identical items", nameof(quantity));

            Quantity = quantity;
        }

        private void ApplyDiscountRules()
        {
            DiscountPercentage = Quantity switch
            {
                >= 10 and <= 20 => 20m,
                >= 4 and < 10 => 10m,
                _ => 0m
            };
        }

        private void CalculateTotalAmount()
        {
            var subtotal = UnitPrice.Multiply(Quantity);
            TotalAmount = subtotal.ApplyDiscount(DiscountPercentage);
        }

        public void UpdateQuantity(int newQuantity)
        {
            SetQuantity(newQuantity);
            ApplyDiscountRules();
            CalculateTotalAmount();
        }

        public void UpdateUnitPrice(Money newUnitPrice)
        {
            UnitPrice = newUnitPrice ?? throw new ArgumentNullException(nameof(newUnitPrice));
            CalculateTotalAmount();
        }
    }
}
