using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ambev.DeveloperEvaluation.Application.Sales.CancelSale
{
    /// <summary>
    /// Command for cancelling a sale
    /// </summary>
    public class CancelSaleCommand : IRequest<CancelSaleResult>
    {
        /// <summary>
        /// The unique identifier of the sale to cancel
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Reason for cancelling the sale (optional)
        /// </summary>
        public string? CancellationReason { get; set; }

        public CancelSaleCommand(Guid id, string? cancellationReason = null)
        {
            Id = id;
            CancellationReason = cancellationReason;
        }
    }
}
