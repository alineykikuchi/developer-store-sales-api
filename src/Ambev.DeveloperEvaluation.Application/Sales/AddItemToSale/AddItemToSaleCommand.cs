using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ambev.DeveloperEvaluation.Application.Sales.AddItemToSale
{
    /// <summary>
    /// Command for adding an item to an existing sale
    /// </summary>
    public class AddItemToSaleCommand : IRequest<AddItemToSaleResult>
    {
        /// <summary>
        /// The unique identifier of the sale to add item to
        /// </summary>
        public Guid SaleId { get; set; }

        /// <summary>
        /// Product information
        /// </summary>
        public AddItemToSaleProduct Product { get; set; } = new();

        /// <summary>
        /// Quantity to add
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// Unit price of the product
        /// </summary>
        public decimal UnitPrice { get; set; }

        /// <summary>
        /// Currency (default: BRL)
        /// </summary>
        public string Currency { get; set; } = "BRL";
    }
}
