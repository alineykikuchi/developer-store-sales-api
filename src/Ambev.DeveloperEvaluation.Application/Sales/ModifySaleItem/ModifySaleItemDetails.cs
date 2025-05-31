using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ambev.DeveloperEvaluation.Application.Sales.ModifySaleItem
{
    /// <summary>
    /// Details of the modified item
    /// </summary>
    public class ModifySaleItemDetails
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string ProductDescription { get; set; } = string.Empty;
        public int PreviousQuantity { get; set; }
        public int NewQuantity { get; set; }
        public decimal PreviousUnitPrice { get; set; }
        public decimal NewUnitPrice { get; set; }
        public string UnitPriceCurrency { get; set; } = string.Empty;
        public decimal PreviousDiscountPercentage { get; set; }
        public decimal NewDiscountPercentage { get; set; }
        public decimal PreviousTotalAmount { get; set; }
        public decimal NewTotalAmount { get; set; }
        public string TotalAmountCurrency { get; set; } = string.Empty;
        public bool QuantityChanged { get; set; }
        public bool PriceChanged { get; set; }
        public bool DiscountChanged { get; set; }
    }
}
