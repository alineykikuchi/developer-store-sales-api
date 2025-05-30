using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ambev.DeveloperEvaluation.Domain.ValueObjects
{
    public class BranchId
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; }
        public string Address { get; private set; }

        public BranchId(Guid id, string name, string address)
        {
            Id = id;
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Address = address ?? throw new ArgumentNullException(nameof(address));
        }
    }
}
