using Ambev.DeveloperEvaluation.Domain.Common;
using Ambev.DeveloperEvaluation.Domain.Enums;
using Ambev.DeveloperEvaluation.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ambev.DeveloperEvaluation.Domain.Entities
{
    public class Sale : BaseEntity
    {
        public string SaleNumber { get; private set; }
        public DateTime SaleDate { get; private set; }
        public CustomerId Customer { get; private set; }
        public BranchId Branch { get; private set; }
        public SaleStatus Status { get; private set; }
        public Money TotalAmount { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? CancelledAt { get; private set; }

        private readonly List<SaleItem> _items = new();
        public IReadOnlyList<SaleItem> Items => _items.AsReadOnly();

        protected Sale() { }

        public Sale(string saleNumber, CustomerId customer, BranchId branch)
        {
            Id = Guid.NewGuid();
            SaleNumber = saleNumber ?? throw new ArgumentNullException(nameof(saleNumber));
            Customer = customer ?? throw new ArgumentNullException(nameof(customer));
            Branch = branch ?? throw new ArgumentNullException(nameof(branch));
            SaleDate = DateTime.UtcNow;
            CreatedAt = DateTime.UtcNow;
            Status = SaleStatus.Active;
            TotalAmount = Money.Zero();
        }

        public void AddItem(ProductId product, int quantity, Money unitPrice)
        {
            if (Status == SaleStatus.Cancelled)
                throw new InvalidOperationException("Cannot add items to a cancelled sale");

            // Verifica se já existe item com o mesmo produto
            var existingItem = _items.FirstOrDefault(i => i.Product.Id == product.Id);
            if (existingItem != null)
            {
                var newQuantity = existingItem.Quantity + quantity;
                if (newQuantity > 20)
                    throw new InvalidOperationException("Cannot have more than 20 identical items in a sale");

                existingItem.UpdateQuantity(newQuantity);
            }
            else
            {
                var newItem = new SaleItem(product, quantity, unitPrice);
                _items.Add(newItem);
            }

            RecalculateTotalAmount();
        }

        public void RemoveItem(Guid itemId)
        {
            if (Status == SaleStatus.Cancelled)
                throw new InvalidOperationException("Cannot remove items from a cancelled sale");

            var item = _items.FirstOrDefault(i => i.Id == itemId);
            if (item == null)
                throw new ArgumentException("Item not found", nameof(itemId));

            _items.Remove(item);
            RecalculateTotalAmount();
        }

        public void UpdateItemQuantity(Guid itemId, int newQuantity)
        {
            if (Status == SaleStatus.Cancelled)
                throw new InvalidOperationException("Cannot update items in a cancelled sale");

            var item = _items.FirstOrDefault(i => i.Id == itemId);
            if (item == null)
                throw new ArgumentException("Item not found", nameof(itemId));

            item.UpdateQuantity(newQuantity);
            RecalculateTotalAmount();
        }

        public void UpdateItemPrice(Guid itemId, Money newUnitPrice)
        {
            if (Status == SaleStatus.Cancelled)
                throw new InvalidOperationException("Cannot update items in a cancelled sale");

            var item = _items.FirstOrDefault(i => i.Id == itemId);
            if (item == null)
                throw new ArgumentException("Item not found", nameof(itemId));

            item.UpdateUnitPrice(newUnitPrice);
            RecalculateTotalAmount();
        }

        public void Cancel()
        {
            if (Status == SaleStatus.Cancelled)
                throw new InvalidOperationException("Sale is already cancelled");

            Status = SaleStatus.Cancelled;
            CancelledAt = DateTime.UtcNow;
        }

        public void Reactivate()
        {
            if (Status == SaleStatus.Active)
                throw new InvalidOperationException("Sale is already active");

            Status = SaleStatus.Active;
            CancelledAt = null;
        }

        private void RecalculateTotalAmount()
        {
            TotalAmount = _items.Aggregate(
                Money.Zero(),
                (total, item) => total.Add(item.TotalAmount)
            );
        }

        // Domain Services podem ser chamados aqui se necessário
        public bool HasDiscountedItems()
        {
            return _items.Any(item => item.DiscountPercentage > 0);
        }

        public int GetTotalItemsCount()
        {
            return _items.Sum(item => item.Quantity);
        }

        public bool IsEligibleForBulkDiscount()
        {
            return _items.Any(item => item.Quantity >= 4);
        }
    }
}
