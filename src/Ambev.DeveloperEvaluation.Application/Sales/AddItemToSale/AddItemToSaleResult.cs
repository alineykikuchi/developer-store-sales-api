using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ambev.DeveloperEvaluation.Application.Sales.AddItemToSale
{
    /// <summary>
    /// Response after adding an item to sale
    /// </summary>
    public class AddItemToSaleResult
    {
        public Guid SaleId { get; set; }
        public string SaleNumber { get; set; } = string.Empty;
        public AddItemToSaleItemResult AddedItem { get; set; } = new();
        public decimal NewSaleTotalAmount { get; set; }
        public string Currency { get; set; } = string.Empty;
        public int TotalItemsCount { get; set; }
        public bool HasDiscountedItems { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
