using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ambev.DeveloperEvaluation.Application.Sales.ModifySaleItem
{
    /// <summary>
    /// Command for modifying an item in an existing sale
    /// </summary>
    public class ModifySaleItemCommand : IRequest<ModifySaleItemResult>
    {
        /// <summary>
        /// The unique identifier of the sale
        /// </summary>
        public Guid SaleId { get; set; }

        /// <summary>
        /// The unique identifier of the item to modify
        /// </summary>
        public Guid ItemId { get; set; }

        /// <summary>
        /// New quantity for the item (optional - if not provided, keeps current)
        /// </summary>
        public int? Quantity { get; set; }

        /// <summary>
        /// New unit price for the item (optional - if not provided, keeps current)
        /// </summary>
        public decimal? UnitPrice { get; set; }

        /// <summary>
        /// Currency for the unit price (default: BRL)
        /// </summary>
        public string Currency { get; set; } = "BRL";
    }
}
