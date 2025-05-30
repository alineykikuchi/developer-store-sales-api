using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ambev.DeveloperEvaluation.Domain.ValueObjects
{
    public class CustomerId
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; } 
        public string Email { get; private set; } 

        public CustomerId(Guid id, string name, string email)
        {
            Id = id;
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Email = email ?? throw new ArgumentNullException(nameof(email));
        }
    }
}
