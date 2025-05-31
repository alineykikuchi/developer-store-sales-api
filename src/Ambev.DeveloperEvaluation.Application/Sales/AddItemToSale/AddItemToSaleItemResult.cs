using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ambev.DeveloperEvaluation.Application.Sales.AddItemToSale
{
    /// <summary>
    /// Information about the added item
    /// </summary>
    public class AddItemToSaleItemResult
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string ProductDescription { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public string UnitPriceCurrency { get; set; } = string.Empty;
        public decimal DiscountPercentage { get; set; }
        public decimal TotalAmount { get; set; }
        public string TotalAmountCurrency { get; set; } = string.Empty;
    }
}
